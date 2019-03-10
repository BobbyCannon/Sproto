#ifndef OSC_ADDRESS_H
#define OSC_ADDRESS_H

#include "OscCommon.h"
#include "OscError.h"
#include <stdbool.h>
#include <stddef.h>

bool OscAddressMatch(const char* oscAddressPattern, const char* const oscAddress);
bool OscAddressMatchPartial(const char* oscAddressPattern, const char* const oscAddress);
bool OscAddressIsLiteral(const char* oscAddressPattern);
unsigned int OscAddressGetNumberOfParts(const char* oscAddressPattern);
OscError OscAddressGetPartAtIndex(const char* oscAddressPattern, const unsigned int index, char* const destination, const size_t destinationSize);

#endif
