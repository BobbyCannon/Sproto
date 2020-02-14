#include "stdafx.h"
#include "CppUnitTest.h"
#include "Shared.hpp"

using namespace Microsoft::VisualStudio::CppUnitTestFramework;

namespace OSCNativeTests
{
	TEST_CLASS(OscMessageTests)
	{
	public:
		TEST_METHOD(ParseMessage)
		{
			// See "Sproto.Tests\OSC\OscMessageTests.cs" -> GetOscMessage for argument list
			char data[] = {	0x2F, 0x41, 0x64, 0x64, 0x72, 0x65, 0x73, 0x73, 0x00, 0x00, 0x00, 0x00, 0x2C, 0x69, 0x75, 0x73, 0x62, 0x68, 0x48, 0x48, 0x74, 0x5B, 0x54, 0x69, 0x73, 0x4E, 0x5D, 0x63, 0x54, 0x46, 0x66, 0x49, 0x49, 0x4E, 0x64, 0x49, 0x49, 0x53, 0x72, 0x6D, 0x70, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x7B, 0x00, 0x00, 0x01, 0xC8, 0x42, 0x6F, 0x6F, 0x6D, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x03, 0x01, 0x02, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x41, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x02, 0x8E, 0xDF, 0xEE, 0xA7, 0x94, 0x00, 0x00, 0x00, 0x00, 0xDF, 0xEE, 0xB4, 0xC4, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x7B, 0x66, 0x6F, 0x78, 0x00, 0x00, 0x00, 0x00, 0x41, 0x42, 0xF6, 0xE6, 0x66, 0x40, 0x4B, 0x29, 0x16, 0x87, 0x2B, 0x02, 0x0C, 0x54, 0x65, 0x73, 0x74, 0x00, 0x00, 0x00, 0x00, 0x01, 0x02, 0x03, 0x04, 0x50, 0x4C, 0x2A, 0x18, 0x00, 0x00, 0x0A, 0x92, 0xE6, 0x1F, 0xB2, 0xA0 };

			auto message = OscMessage();
			auto result = OscMessageInitializeFromCharArray(&message, &data[0], sizeof(data));
			Assert::AreEqual(OscErrorNone, result);
			Assert::IsTrue(OscContentsIsMessage(&message), L"The packet is not a message...");
			Assert::AreEqual(28, (int) message.argumentCount);

			Assert::AreEqual(size_t(8), message.oscAddressPatternLength);
			Assert::AreEqual('/', message.oscAddressPattern[0]);
			Assert::AreEqual('A', message.oscAddressPattern[1]);
			Assert::AreEqual('d', message.oscAddressPattern[2]);
			Assert::AreEqual('d', message.oscAddressPattern[3]);
			Assert::AreEqual('r', message.oscAddressPattern[4]);
			Assert::AreEqual('e', message.oscAddressPattern[5]);
			Assert::AreEqual('s', message.oscAddressPattern[6]);
			Assert::AreEqual('s', message.oscAddressPattern[7]);

			int iActual;
			result = OscMessageGetArgumentAsInt32(&message, &iActual);
			Assert::AreEqual(OscErrorNone, result);
			Assert::AreEqual(123, iActual);

			unsigned int uiActual;
			result = OscMessageGetArgumentAsUInt32(&message, &uiActual);
			Assert::AreEqual(OscErrorNone, result);
			Assert::AreEqual(456u, uiActual);

			char buffer[128];
			size_t bufferSize;
			result = OscMessageGetArgumentAsString(&message, buffer, 128);
			Assert::AreEqual(OscErrorNone, result);
			Assert::AreEqual("Boom", buffer);

			result = OscMessageGetArgumentAsBlob(&message, &bufferSize, buffer, 128);
			Assert::AreEqual(OscErrorNone, result);
			Assert::AreEqual((size_t) 3, bufferSize);
			Assert::AreEqual(1, (int) buffer[0]);
			Assert::AreEqual(2, (int) buffer[1]);
			Assert::AreEqual(3, (int) buffer[2]);

			int64_t lValue;
			result = OscMessageGetArgumentAsInt64(&message, &lValue);
			Assert::AreEqual(OscErrorNone, result);
			Assert::AreEqual((int64_t) 321, lValue);

			uint64_t ulValue;
			result = OscMessageGetArgumentAsUInt64(&message, &ulValue);
			Assert::AreEqual(OscErrorNone, result);
			Assert::AreEqual((uint64_t) 654, ulValue);

			OscTimeTag time;
			result = OscMessageGetArgumentAsTimeTag(&message, &time);
			Assert::AreEqual(OscErrorNone, result);
			Assert::AreEqual((uint64_t) 16136018769012064256, time.value);

			OscTimeTag time2;
			result = OscMessageGetArgumentAsTimeTag(&message, &time2);
			Assert::AreEqual(OscErrorNone, result);
			Assert::AreEqual((uint64_t) 16136033268821655552, time2.value);

			// Skip start of array
			auto type = OscMessageGetArgumentType(&message);
			Assert::AreEqual(OscTypeTagBeginArray, type);
			OscMessageSkipArgument(&message);

			bool bValue;
			OscMessageGetArgumentAsBool(&message, &bValue);
			Assert::AreEqual(true, bValue);

			result = OscMessageGetArgumentAsInt32(&message, &iActual);
			Assert::AreEqual(OscErrorNone, result);
			Assert::AreEqual(123, iActual);

			result = OscMessageGetArgumentAsString(&message, buffer, 128);
			Assert::AreEqual(OscErrorNone, result);
			Assert::AreEqual("fox", buffer);

			type = OscMessageGetArgumentType(&message);
			Assert::AreEqual(OscTypeTagNil, type);
			OscMessageSkipArgument(&message);

			// Skip end of array
			type = OscMessageGetArgumentType(&message);
			Assert::AreEqual(OscTypeTagEndArray, type);
			OscMessageSkipArgument(&message);

			result = OscMessageGetArgumentAsCharacter(&message, &buffer[0]);
			Assert::AreEqual(OscErrorNone, result);
			Assert::AreEqual('A', buffer[0]);

			result = OscMessageGetArgumentAsBool(&message, &bValue);
			Assert::AreEqual(OscErrorNone, result);
			Assert::AreEqual(true, bValue);

			result = OscMessageGetArgumentAsBool(&message, &bValue);
			Assert::AreEqual(OscErrorNone, result);
			Assert::AreEqual(false, bValue);

			float fValue;
			result = OscMessageGetArgumentAsFloat32(&message, &fValue);
			Assert::AreEqual(OscErrorNone, result);
			Assert::AreEqual(123.45f, fValue);

			type = OscMessageGetArgumentType(&message);
			Assert::AreEqual(OscTypeTagInfinitum, type);
			OscMessageSkipArgument(&message);

			type = OscMessageGetArgumentType(&message);
			Assert::AreEqual(OscTypeTagInfinitum, type);
			OscMessageSkipArgument(&message);

			type = OscMessageGetArgumentType(&message);
			Assert::AreEqual(OscTypeTagNil, type);
			OscMessageSkipArgument(&message);

			double dValue;
			result = OscMessageGetArgumentAsDouble(&message, &dValue);
			Assert::AreEqual(OscErrorNone, result);
			Assert::AreEqual(54.321, dValue);

			type = OscMessageGetArgumentType(&message);
			Assert::AreEqual(OscTypeTagInfinitum, type);
			OscMessageSkipArgument(&message);

			type = OscMessageGetArgumentType(&message);
			Assert::AreEqual(OscTypeTagInfinitum, type);
			OscMessageSkipArgument(&message);

			result = OscMessageGetString(&message, buffer, 128);
			Assert::AreEqual(OscErrorNone, result);
			Assert::AreEqual("Test", buffer);

			RgbaColor color;
			result = OscMessageGetRgbaColor(&message, &color);
			Assert::AreEqual(OscErrorNone, result);
			Assert::AreEqual((char) 1, color.red);
			Assert::AreEqual((char) 2, color.green);
			Assert::AreEqual((char) 3, color.blue);
			Assert::AreEqual((char) 4, color.alpha);

			MidiMessage midi;
			result = OscMessageGetArgumentAsMidiMessage(&message, &midi);
			Assert::AreEqual(OscErrorNone, result);
			Assert::AreEqual((char) 80, midi.portID);
			Assert::AreEqual((char) 76, midi.status);
			Assert::AreEqual((char) 42, midi.data1);
			Assert::AreEqual((char) 24, midi.data2);

			type = OscMessageGetArgumentType(&message);
			Assert::AreEqual(OscTypeTagTimeSpan, type);
			OscMessageSkipArgument(&message);
			// todo: add support time span;
		}
	};
}
