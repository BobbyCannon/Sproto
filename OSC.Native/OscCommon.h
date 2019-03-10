#ifndef OSC_COMMON_H
#define OSC_COMMON_H

#ifdef OSC_EMBEDDED
#define OSC_PACKED __attribute__((__packed__))
#else
#define OSC_PACKED
#endif

#include <float.h>
#include <stdbool.h>
#include <stdint.h>

/*
 * @brief Comment out this definition if the platform is big-endian.  For
 * example: Arduino, Atmel AVR, Microchip PIC, Intel x86-64 are little-endian.
 * See http://en.wikipedia.org/wiki/Endianness
 */
#define LITTLE_ENDIAN_PLATFORM

/*
 * @brief Maximum packet size permitted by the transport layer.  Reducing this
 * value will reduce the amount of memory required.
 */
#define MAX_TRANSPORT_SIZE (4096)

/*
 * @brief Comment out this definition to prevent the OscErrorGetMessage function
 * from providing detailed error messages.  This will reduce the amount of
 * memory required.
 */
#define OSC_ERROR_MESSAGES_ENABLED

/*
 * @brief 32-bit RGBA colour.
 * See http://en.wikipedia.org/wiki/RGBA_color_space
 */
typedef struct OSC_PACKED {
	#ifdef LITTLE_ENDIAN_PLATFORM
	char alpha; // LSB
	char blue;
	char green;
	char red; // MSB
	#else
    char red; // MSB
    char green;
    char blue;
    char alpha; // LSB
	#endif
} RgbaColour;

/*
 * @brief 4 byte MIDI message as described in OSC 1.0 specification.
 */
typedef struct OSC_PACKED {
	#ifdef LITTLE_ENDIAN_PLATFORM
	char data2; // LSB
	char data1;
	char status;
	char portID; // MSB
	#else
    char portID; // MSB
    char status;
    char data1;
    char data2; // LSB
	#endif
} MidiMessage;

/*
 * @brief Union of all 32-bit OSC argument types defined in OSC 1.0
 * specification.
 */
typedef union
{
	int32_t int32;
	float float32;
	RgbaColour rgbaColour;
	MidiMessage midiMessage;

	struct OSC_PACKED {
		#ifdef LITTLE_ENDIAN_PLATFORM
		char byte0; // LSB
		char byte1;
		char byte2;
		char byte3; // MSB
		#else
        char byte3; // MSB
        char byte2;
        char byte1;
        char byte0; // LSB
		#endif
	}
	byteStruct;
} OscArgument32;

/*
 * @brief OSC time tag.  Same representation used by NTP timestamps.
 */
typedef union
{
	uint64_t value;

	struct OSC_PACKED
	{
		uint32_t fraction;
		uint32_t seconds;
	} dwordStruct;

	struct OSC_PACKED
	{
		#ifdef LITTLE_ENDIAN_PLATFORM
		char byte0; // LSB
		char byte1;
		char byte2;
		char byte3;
		char byte4;
		char byte5;
		char byte6;
		char byte7; // MSB
		#else
        char byte7; // MSB
        char byte6;
        char byte5;
        char byte4;
        char byte3;
        char byte2;
        char byte1;
        char byte0; // LSB
		#endif
	} byteStruct;
} OscTimeTag;

/*
 * @brief 64-bit double.  Defined as double or long double depending on
 * platform.
 */
#if (DBL_MANT_DIG == 53)
typedef double Double64;
#else
typedef long double Double64; // use long double if double is not 64-bit
#endif

/*
 * @brief Union of all 64-bit OSC argument types defined in OSC 1.0
 * specification.
 */
typedef union
{
	uint64_t int64;
	OscTimeTag oscTimeTag;
	Double64 double64;

	struct OSC_PACKED {
		#ifdef LITTLE_ENDIAN_PLATFORM
		char byte0; // LSB
		char byte1;
		char byte2;
		char byte3;
		char byte4;
		char byte5;
		char byte6;
		char byte7; // MSB
		#else
        char byte7; // MSB
        char byte6;
        char byte5;
        char byte4;
        char byte3;
        char byte2;
        char byte1;
        char byte0; // LSB
		#endif
	}
	byteStruct;
} OscArgument64;

extern const OscTimeTag oscTimeTagZero;

uint16_t OscCalculateCrc16(const uint8_t* data, int length);
bool OscContentsIsMessage(const void* oscContents);
bool OscContentsIsBundle(const void* oscPacket);
bool OscExtendedBundleIsValid(const void* oscContents);

#endif
