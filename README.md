![logo](/Sproto.png)

# sproto

.NET standard (C#) implementation of protocol that work well for serial interfaces:

* OSC with support for SLIP processing.

Code inspired from the following other MIT repositories

* https://github.com/xioTechnologies/OSC99
* https://github.com/ValdemarOrn/SharpOSC

### OSC

Support for the following core data types

* boolean (T,F)
* char (c)
* byte[] (b)
* long (h)
* unsigned long (H)+
* integer (i)
* unsigned integer (u)+
* double (I,d)
* float (f)
* string (s)
* null (N)
* OscCrc (C)
* OscRgba (r)
* OscMidi (m)
* OscSymbol (S)
* OscTime (t)

\+ Additions to OSC 1.0 specification

Support for the following additional data types.

* byte (c), Guid (s), DateTime (t)

### References

* http://opensoundcontrol.org/spec-1_0
* https://tools.ietf.org/html/rfc1055
