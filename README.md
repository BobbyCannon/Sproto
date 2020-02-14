![logo](/Sproto.png)

# sproto

.NET standard (C#) implementation of protocol that work well for serial interfaces:

* OSC with support for SLIP processing.

### OSC

OSC 1.0 specification support

* boolean (T,F)
* char (c)
* long (h)
* integer (i)
* double (I,d)
* float (f)
* string (s)
* null (N)
* OscBlob (b)
* OscCrc (C)
* OscRgba (r)
* OscMidi (m)
* OscSymbol (S)
* OscTime (t)

C / C# Additions

* unsigned integer (u)
* unsigned long (H)

C# Additions

* byte (c)
* Guid (s)
* DateTime (t)
* TimeSpan (p)

*Custom Data Types*

Samples for adding custom OSC types can be found in the Sproto.Tests project. See "SampleCustomValue.cs" for an example used in "SampleOscCommand.cs" command.

### References

* http://opensoundcontrol.org/spec-1_0
* https://tools.ietf.org/html/rfc1055

### Special Thanks

to the following other MIT repositories

* https://github.com/xioTechnologies/OSC99
* https://github.com/ValdemarOrn/SharpOSC
