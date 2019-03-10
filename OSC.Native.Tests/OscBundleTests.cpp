#include "stdafx.h"
#include "CppUnitTest.h"
#include "Shared.hpp"

using namespace Microsoft::VisualStudio::CppUnitTestFramework;

namespace OSCNativeTests
{
	TEST_CLASS(OscBundleTests)
	{
	public:

		TEST_METHOD(ParseBundle)
		{
			// new OscBundle(time.Value, new OscMessage("/message", 123, "foo", true, null), new OscMessage("/delay", 321));
			uint8_t data[] = {
				0x23, 0x62, 0x75, 0x6E, 0x64, 0x6C, 0x65, 0x00, 0xDF, 0xEE, 0xB4, 0xC4, 0x00, 0x00, 0x00, 0x00, 0x00,
				0x00, 0x00, 0x1C, 0x2F, 0x6D, 0x65, 0x73, 0x73, 0x61, 0x67, 0x65, 0x00, 0x00, 0x00, 0x00, 0x2C, 0x69,
				0x73, 0x54, 0x4E, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x7B, 0x66, 0x6F, 0x6F, 0x00, 0x00, 0x00, 0x00,
				0x10, 0x2F, 0x64, 0x65, 0x6C, 0x61, 0x79, 0x00, 0x00, 0x2C, 0x69, 0x00, 0x00, 0x00, 0x00, 0x01, 0x41
			};

			auto packet = OscPacket();
			auto result = OscPacketInitializeFromCharArray(&packet, (char *) &data, sizeof(data));
			Assert::AreEqual(OscErrorNone, result);

			Assert::IsTrue(OscContentsIsBundle(&packet), L"The packet is not a bundle...");

			auto bundle = (OscBundle *) &packet;
			Assert::AreEqual("#bundle\0", bundle->header);
			Assert::AreEqual(0, (int) bundle->oscBundleElementsIndex);

			auto element = OscBundleElement();
			result = OscBundleGetBundleElement(bundle, &element);
			Assert::AreEqual(OscErrorNone, result);
			Assert::AreEqual(32, (int) bundle->oscBundleElementsIndex);

			// Process the first message
			auto message = OscMessage();
			result = OscMessageInitializeFromCharArray(&message, (char *) element.contents, element.size.int32);
			Assert::AreEqual(OscErrorNone, result);
			Assert::AreEqual("/message", message.oscAddressPattern);
			Assert::AreEqual(8, (int) message.argumentsSize);

			auto messageArgumentType = OscMessageGetArgumentType(&message);
			Assert::AreEqual(OscTypeTagInt32, messageArgumentType);
			int32_t messageArgumentInt;
			result = OscMessageGetArgumentAsInt32(&message, &messageArgumentInt);
			Assert::AreEqual(OscErrorNone, result);
			Assert::AreEqual(123, messageArgumentInt);

			messageArgumentType = OscMessageGetArgumentType(&message);
			Assert::AreEqual(OscTypeTagString, messageArgumentType);
			char buffer[32];
			result = OscMessageGetArgumentAsString(&message, &buffer[0], 32);
			Assert::AreEqual(OscErrorNone, result);
			Assert::AreEqual("foo", buffer);

			messageArgumentType = OscMessageGetArgumentType(&message);
			Assert::AreEqual(OscTypeTagTrue, messageArgumentType);
			result = OscMessageSkipArgument(&message);
			Assert::AreEqual(OscErrorNone, result);

			messageArgumentType = OscMessageGetArgumentType(&message);
			Assert::AreEqual(OscTypeTagNil, messageArgumentType);

			// Process the second message
			auto element2 = OscBundleElement();
			result = OscBundleGetBundleElement(bundle, &element2);
			Assert::AreEqual(OscErrorNone, result);

			auto message2 = OscMessage();
			result = OscMessageInitializeFromCharArray(&message2, (char *) element2.contents, element2.size.int32);
			Assert::AreEqual(OscErrorNone, result);

			Assert::AreEqual("/delay", message2.oscAddressPattern);
			Assert::AreEqual(4, (int) message2.argumentsSize);

			messageArgumentType = OscMessageGetArgumentType(&message2);
			Assert::AreEqual(OscTypeTagInt32, messageArgumentType);
			result = OscMessageGetArgumentAsInt32(&message2, &messageArgumentInt);
			Assert::AreEqual(OscErrorNone, result);
			Assert::AreEqual(321, messageArgumentInt);
		}

		TEST_METHOD(ParseExtendedBundle)
		{
			// new OscBundle(time.Value, new OscMessage("/message", 123, "foo", true, null), new OscMessage("/delay", 321));
			uint8_t data[] = {
				0x2B, 0x62, 0x75, 0x6E, 0x64, 0x6C, 0x65, 0x00, 0xDF, 0xEE, 0xB4, 0xC4, 0x00, 0x00, 0x00, 0x00, 0x00,
				0x00, 0x00, 0x1C, 0x2F, 0x6D, 0x65, 0x73, 0x73, 0x61, 0x67, 0x65, 0x00, 0x00, 0x00, 0x00, 0x2C, 0x69,
				0x73, 0x54, 0x4E, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x7B, 0x62, 0x61, 0x72, 0x00, 0x00, 0x00, 0x00,
				0x10, 0x2F, 0x64, 0x65, 0x6C, 0x61, 0x79, 0x00, 0x00, 0x2C, 0x69, 0x00, 0x00, 0x00, 0x00, 0x01, 0x41,
				0x45, 0x9C, 0x00, 0x00
			};

			auto packet = OscPacket();
			auto result = OscPacketInitializeFromCharArray(&packet, (char *) &data, sizeof(data));
			Assert::AreEqual(OscErrorNone, result);
			Assert::IsTrue(OscContentsIsBundle(&packet), L"The packet is not a bundle...");
			Assert::IsTrue(OscExtendedBundleIsValid(&packet), L"The extended bundle is not valid...");

			auto bundle = (OscBundle *) &packet;
			Assert::AreEqual("+bundle\0", bundle->header);
			Assert::AreEqual(0, (int) bundle->oscBundleElementsIndex);

			auto element = OscBundleElement();
			result = OscBundleGetBundleElement(bundle, &element);
			Assert::AreEqual(OscErrorNone, result);
			Assert::AreEqual(32, (int) bundle->oscBundleElementsIndex);

			// Process the first message
			auto message = OscMessage();
			result = OscMessageInitializeFromCharArray(&message, (char *) element.contents, element.size.int32);
			Assert::AreEqual(OscErrorNone, result);
			Assert::AreEqual("/message", message.oscAddressPattern);
			Assert::AreEqual(8, (int) message.argumentsSize);
			Assert::AreEqual(4, (int) message.argumentCount);

			auto messageArgumentType = OscMessageGetArgumentType(&message);
			Assert::AreEqual(OscTypeTagInt32, messageArgumentType);
			int32_t messageArgumentInt;
			result = OscMessageGetArgumentAsInt32(&message, &messageArgumentInt);
			Assert::AreEqual(OscErrorNone, result);
			Assert::AreEqual(123, messageArgumentInt);

			messageArgumentType = OscMessageGetArgumentType(&message);
			Assert::AreEqual(OscTypeTagString, messageArgumentType);
			char buffer[32];
			result = OscMessageGetArgumentAsString(&message, &buffer[0], 32);
			Assert::AreEqual(OscErrorNone, result);
			Assert::AreEqual("bar", buffer);

			messageArgumentType = OscMessageGetArgumentType(&message);
			Assert::AreEqual(OscTypeTagTrue, messageArgumentType);
			result = OscMessageSkipArgument(&message);
			Assert::AreEqual(OscErrorNone, result);

			messageArgumentType = OscMessageGetArgumentType(&message);
			Assert::AreEqual(OscTypeTagNil, messageArgumentType);

			// Process the second message
			auto element2 = OscBundleElement();
			result = OscBundleGetBundleElement(bundle, &element2);
			Assert::AreEqual(OscErrorNone, result);

			auto message2 = OscMessage();
			result = OscMessageInitializeFromCharArray(&message2, (char *) element2.contents, element2.size.int32);
			Assert::AreEqual(OscErrorNone, result);

			Assert::AreEqual("/delay", message2.oscAddressPattern);
			Assert::AreEqual(4, (int) message2.argumentsSize);
			Assert::AreEqual(1, (int) message2.argumentCount);

			messageArgumentType = OscMessageGetArgumentType(&message2);
			Assert::AreEqual(OscTypeTagInt32, messageArgumentType);
			result = OscMessageGetArgumentAsInt32(&message2, &messageArgumentInt);
			Assert::AreEqual(OscErrorNone, result);
			Assert::AreEqual(321, messageArgumentInt);
		}

		TEST_METHOD(ParseExtendedBundleShouldFailCrcCheck)
		{
			uint8_t data[] =
			{
				0x2B, 0x62, 0x75, 0x6E, 0x64, 0x6C, 0x65, 0x00, 0xDF, 0xEE, 0xB4, 0xC4, 0x00, 0x00, 0x00, 0x00, 0x00,
				0x00, 0x00, 0x1C, 0x2F, 0x6D, 0x65, 0x73, 0x73, 0x61, 0x67, 0x65, 0x00, 0x00, 0x00, 0x00, 0x2C, 0x69,
				0x73, 0x54, 0x4E, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x7B, 0x62, 0x61, 0x72, 0x00, 0x00, 0x00, 0x00,
				0x10, 0x2F, 0x64, 0x65, 0x6C, 0x61, 0x79, 0x00, 0x00, 0x2C, 0x69, 0x00, 0x00, 0x00, 0x00, 0x01, 0x41,
				0x45, 0x9C, 0x00, 0x00
			};

			auto packet = OscPacket();
			auto result = OscPacketInitializeFromCharArray(&packet, (char *) &data, sizeof(data));
			Assert::AreEqual(OscErrorNone, result);
			Assert::IsTrue(OscContentsIsBundle(&packet), L"The packet is not a bundle...");
			Assert::IsTrue(OscExtendedBundleIsValid(&packet), L"The extended bundle is not valid...");

			auto dataLength = sizeof(packet.contents);
			auto randomOffset = rand() % (dataLength - 1);
			auto randomBit = 1 << (rand() % 8);
			packet.contents[randomOffset] = (packet.contents[randomOffset] & randomBit) == randomBit
												? packet.contents[randomOffset] ^ randomBit
												: packet.contents[randomOffset] | randomBit;

			Assert::IsTrue(OscContentsIsBundle(&packet), L"The packet is not a bundle...");
			Assert::IsFalse(OscExtendedBundleIsValid(&packet), L"The extended bundle should not be valid...");
		}

		TEST_METHOD(ToBytesForBundle)
		{
			uint8_t expected[]
			{
				0x23, 0x62, 0x75, 0x6E, 0x64, 0x6C, 0x65, 0x00, 0xDF, 0xEE, 0xB4, 0xC4, 0x00, 0x00, 0x00, 0x00, 0x00,
				0x00, 0x00, 0x1C, 0x2F, 0x6D, 0x65, 0x73, 0x73, 0x61, 0x67, 0x65, 0x00, 0x00, 0x00, 0x00, 0x2C, 0x69,
				0x73, 0x54, 0x4E, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x7B, 0x62, 0x61, 0x72, 0x00, 0x00, 0x00, 0x00,
				0x10, 0x2F, 0x64, 0x65, 0x6C, 0x61, 0x79, 0x00, 0x00, 0x2C, 0x69, 0x00, 0x00, 0x00, 0x00, 0x01, 0x41
			};

			auto bundle = OscBundle();
			OscTimeTag time = {16136033268821655552u};

			OscBundleInitialize(&bundle, time);

			auto message1 = OscMessage();
			OscMessageInitialize(&message1, "/message");
			OscMessageAddInt32(&message1, 123);
			OscMessageAddString(&message1, "bar");
			OscMessageAddBool(&message1, true);
			OscMessageAddNil(&message1);
			OscBundleAddContents(&bundle, &message1);

			auto message2 = OscMessage();
			OscMessageInitialize(&message2, "/delay");
			OscMessageAddInt32(&message2, 321);
			OscBundleAddContents(&bundle, &message2);

			auto packet = OscPacket();
			auto result = OscPacketInitializeFromContents(&packet, &bundle);
			Assert::AreEqual(OscErrorNone, result);
			Assert::AreEqual(68, (int) packet.size);

			auto result2 = memcmp(packet.contents, expected, 68);
			Assert::AreEqual(0, result2, L"The contents are not equal...");
		}

		TEST_METHOD(ToBytesForExtendedBundle)
		{
			uint8_t expected[]
			{
				0x2B, 0x62, 0x75, 0x6E, 0x64, 0x6C, 0x65, 0x00, 0xDF, 0xEE, 0xB4, 0xC4, 0x00, 0x00, 0x00, 0x00, 0x00,
				0x00, 0x00, 0x1C, 0x2F, 0x6D, 0x65, 0x73, 0x73, 0x61, 0x67, 0x65, 0x00, 0x00, 0x00, 0x00, 0x2C, 0x69,
				0x73, 0x54, 0x4E, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x7B, 0x62, 0x61, 0x72, 0x00, 0x00, 0x00, 0x00,
				0x10, 0x2F, 0x64, 0x65, 0x6C, 0x61, 0x79, 0x00, 0x00, 0x2C, 0x69, 0x00, 0x00, 0x00, 0x00, 0x01, 0x41,
				0x45, 0x9C, 0x00, 0x00
			};

			auto bundle = OscBundle();
			OscTimeTag time = {16136033268821655552u};

			OscBundleInitializeExtended(&bundle, time);

			auto message1 = OscMessage();
			OscMessageInitialize(&message1, "/message");
			OscMessageAddInt32(&message1, 123);
			OscMessageAddString(&message1, "bar");
			OscMessageAddBool(&message1, true);
			OscMessageAddNil(&message1);
			OscBundleAddContents(&bundle, &message1);

			auto message2 = OscMessage();
			OscMessageInitialize(&message2, "/delay");
			OscMessageAddInt32(&message2, 321);
			OscBundleAddContents(&bundle, &message2);

			auto packet = OscPacket();
			auto result = OscPacketInitializeFromContents(&packet, &bundle);
			Assert::AreEqual(OscErrorNone, result);
			Assert::AreEqual(72, (int) packet.size);
			Assert::IsTrue(OscExtendedBundleIsValid(&packet), L"The extended bundle is not valid...");

			auto result2 = memcmp(packet.contents, expected, 72);
			Assert::AreEqual(0, result2, L"The contents are not equal...");
		}
	};
}
