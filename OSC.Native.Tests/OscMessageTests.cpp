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
			char data[] = {
				0x2F, 0x41, 0x64, 0x64, 0x72, 0x65, 0x73, 0x73, 0x00, 0x00, 0x00, 0x00, 0x2C, 0x69, 0x66, 0x73, 0x62, 0x68, 0x74,
				0x74, 0x64, 0x5B, 0x54, 0x69, 0x73, 0x4E, 0x5D, 0x63, 0x54, 0x46, 0x49, 0x49, 0x4E, 0x49, 0x49, 0x53, 0x72, 0x6D,
				0x00, 0x00, 0x00, 0x00, 0x00, 0x7B, 0x42, 0xF6, 0xE6, 0x66, 0x42, 0x6F, 0x6F, 0x6D, 0x00, 0x00, 0x00, 0x00, 0x00,
				0x00, 0x00, 0x03, 0x01, 0x02, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x41, 0xDF, 0xEE, 0xA7, 0x94,
				0x00, 0x00, 0x00, 0x00, 0xDF, 0xEE, 0xB4, 0xC4, 0x00, 0x00, 0x00, 0x00, 0x40, 0x4B, 0x29, 0x16, 0x87, 0x2B, 0x02,
				0x0C, 0x00, 0x00, 0x00, 0x7B, 0x66, 0x6F, 0x78, 0x00, 0x00, 0x00, 0x00, 0x41, 0x54, 0x65, 0x73, 0x74, 0x00, 0x00,
				0x00, 0x00, 0x01, 0x02, 0x03, 0x04, 0x50, 0x4C, 0x2A, 0x18
			};

			auto message = OscMessage();
			auto result = OscMessageInitializeFromCharArray(&message, &data[0], sizeof(data));
			Assert::AreEqual(OscErrorNone, result);
			Assert::IsTrue(OscContentsIsMessage(&message), L"The packet is not a message...");
			Assert::AreEqual(25, (int) message.argumentCount);

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
			OscMessageGetArgumentAsInt32(&message, &iActual);
			Assert::AreEqual(123, iActual);

			float fActual;
			OscMessageGetArgumentAsFloat32(&message, &fActual);
			Assert::AreEqual((float) 123.45, fActual);

			char sActual[128];
			OscMessageGetArgumentAsString(&message, sActual, sizeof(sActual));
			Assert::AreEqual("Boom", sActual);

			size_t sActualSize;
			OscMessageGetArgumentAsBlob(&message, &sActualSize, sActual, sizeof(sActual));
			Assert::AreEqual((size_t) 3, sActualSize);
			Assert::AreEqual((char) 1, sActual[0]);
			Assert::AreEqual((char) 2, sActual[1]);
			Assert::AreEqual((char) 3, sActual[2]);

			
		}
	};
}
