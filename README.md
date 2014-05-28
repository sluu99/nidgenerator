NidGenerator
============

A simpler version of Twitter's [Snowflake] (https://github.com/twitter/snowflake) ID generator which allows IDs to be generated locally instead of making a remote call.

The ID generator constructor takes in an instance ID and a timestamp.
The timestamp can be used to make sure that all the instances have the same source of time (i.e. using the [NTicket](https://github.com/sluu99/nticket) timestamp).

Each ID is made of 63 bits:

- 41 bits timestamp at millisecond precision (custom epoch us 2014-01-01 UTC)
- 10 bits instance ID
- 12 bits sequence ID
