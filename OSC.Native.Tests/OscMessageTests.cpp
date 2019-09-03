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
			char data[] = {
				0x2F, 0x74, 0x65, 0x73, 0x74, 0x00, 0x00, 0x00, 0x2C, 0x54, 0x69, 0x00, 0x00, 0x00, 0x00, 0x7B
			};

			auto message = OscMessage();
			auto result = OscMessageInitializeFromCharArray(&message, &data[0], sizeof(data));
			Assert::AreEqual(OscErrorNone, result);
			Assert::IsTrue(OscContentsIsMessage(&message), L"The packet is not a message...");
			Assert::AreEqual(2, (int) message.argumentCount);

			bool bActual;
			int iActual;
			OscMessageGetArgumentAsBool(&message, &bActual);
			OscMessageGetArgumentAsInt32(&message, &iActual);

			Assert::AreEqual(size_t(5), message.oscAddressPatternLength);
			Assert::AreEqual('/', message.oscAddressPattern[0]);
			Assert::AreEqual('t', message.oscAddressPattern[1]);
			Assert::AreEqual('e', message.oscAddressPattern[2]);
			Assert::AreEqual('s', message.oscAddressPattern[3]);
			Assert::AreEqual('t', message.oscAddressPattern[4]);
			Assert::AreEqual(true, bActual);
			Assert::AreEqual(123, iActual);
		}
	};
}
