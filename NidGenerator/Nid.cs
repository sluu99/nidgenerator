using System;

namespace NidGenerator
{
    public class Nid : INid
    {
        static readonly DateTime CustomEpoch = new DateTime(2014, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        static readonly uint SequenceMax = 0xFFF; // 12 bits
        static readonly uint InstanceIdMax = 0x3FF; // 10 bits

        public Nid(uint instanceId, DateTime currentUtc)
        {
            if (instanceId < 0 || instanceId > InstanceIdMax)
            {
                throw new ArgumentOutOfRangeException("instanceId", "Instance ID must be inclusive between 0 and 1023");
            }

            if (DateTime.UtcNow <= CustomEpoch)
            {
                throw new ApplicationException(string.Format("The system clock must be set to after {0}", CustomEpoch));
            }

            this.padLock = new object();
            this.lastTimestamp = 0;
            this.sequence = 0;
            this.instanceId = instanceId;
            this.epochOffset = (ulong)(DateTime.UtcNow - currentUtc).TotalMilliseconds;
        }

        private uint instanceId;
        private ulong epochOffset;
        private ulong lastTimestamp;
        private uint sequence;
        private object padLock;

        public long NextId()
        {
            lock (this.padLock)
            {
                // put sequence condition first to make sure it's short circuited
                while (this.sequence > SequenceMax && GetTimestamp() == this.lastTimestamp)
                {
                }

                ulong timestamp = GetTimestamp();

                if (timestamp != this.lastTimestamp)
                {
                    this.sequence = 0;
                    this.lastTimestamp = timestamp;
                }
                else
                {
                    this.sequence++;
                }

                ulong id = timestamp << 10;
                id |= instanceId;
                id <<= 12;
                id |= this.sequence;

                return (long)id;
            }
        }

        ulong GetTimestamp()
        {
            return ((ulong)(DateTime.UtcNow - CustomEpoch).TotalMilliseconds) + this.epochOffset;
        }
    }
}
