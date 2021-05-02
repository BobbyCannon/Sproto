#region References

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#endregion

namespace Sproto.CodeGenerator
{
	[TestClass]
	public class CodeGenerator
	{
		#region Methods

		[TestMethod]
		public void Generate()
		{
			var test = @"GL - GLONASS Receiver
GP - Global Positioning System (GPS)
Heading Track Controller (Autopilot): General - AG, Magnetic - AP
AI - Automatic Identification System
CD - Digital Selective Calling (DSC)
CR - Data Receiver
CS - Satellite
CT - Radio-Telephone (MF/HF)
CV - Radio-Telephone (VHF)
CX - Scanning Receiver
DE - DECCA Navigator
DF - Direction Finder
EC - Electronic Chart System (ECS)
EI - Electronic Chart Display & Information System (ECDIS)
EP - Emergency Position Indicating Beacon (EPIRB)
ER - Engine room Monitoring Systems
GN - Global Navigation Satellite System (GNSS)
HC - HEADING SENSORS: Compass, Magnetic
HE - Gyro, North Seeking
HN - Gyro, Non-North Seeking
II - Integrated Instrumentation
IN - Integrated Navigation
LC - Loran C
P - Proprietary Code
RA - Radar and/or Radar Plotting
SD - Sounder, depth
SN - Electronic Positioning System, other/general
SS - Sounder, scanning
TI - Turn Rate Indicator
VD - VELOCITY SENSORS: Doppler, other/general
VM - Speed Log, Water, Magnetic
VW - Speed Log, Water, Mechanical
VR - Voyage Data Recorder
YX - Transducer
ZA - TIMEKEEPERS, TIME/DATE: Atomic Clock
ZC - Chronometer
ZQ - Quartz
ZV - Radio Update
WI - Weather Instruments";

			var lines = test.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

			foreach (var line in lines)
			{
				var parts = line.Split(new[] { " - " }, StringSplitOptions.RemoveEmptyEntries);
				if (parts.Length != 2)
				{
					$"***** {line}".Dump();
					continue;
				}

				$"[Display(ShortName = \"{parts[0]}\", Name = \"{parts[1]}\")]".Dump();
				(parts[1].Replace(" ", "") + ",").Dump();
			}
		}

		#endregion
	}
}