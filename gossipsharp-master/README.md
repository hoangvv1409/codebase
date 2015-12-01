Overview
========

GossipSharp is an implementation of a peer to peer gossip protocol.

Some characteristics of the protocol are:

- messages are transferred directly peer to peer using TCP
- authentication through a common cluster key (in form of a secret string)
- using protocol buffers for message serialization/deserialization
- it is very lightweight and performant

What the protocol DOES NOT support:

- cluster routing (nodes have to have direct TCP connections to each other)
- encryption (however the common cluster is safely hashed)

See also:
---------

[MassTransit - Service Bus for .NET](http://masstransit-project.com/)
