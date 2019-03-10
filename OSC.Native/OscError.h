#ifndef OSC_ERROR_H
#define OSC_ERROR_H

/*
 * @brief Enumerated error codes for debugging and user feedback.
 */
typedef enum
{
	OscErrorNone = 0,

	/* Common errors  */
	OscErrorDestinationTooSmall,
	OscErrorSizeIsNotMultipleOfFour,
	OscErrorCallbackFunctionUndefined,

	/* OscAddress errors  */
	OscErrorNotEnoughPartsInAddressPattern,

	/* OscMessage errors  */
	OscErrorNoSlashAtStartOfMessage,
	OscErrorAddressPatternTooLong,
	OscErrorTooManyArguments,
	OscErrorArgumentsSizeTooLarge,
	OscErrorUndefinedAddressPattern,
	OscErrorMessageSizeTooSmall,
	OscErrorMessageSizeTooLarge,
	OscErrorSourceEndsBeforeEndOfAddressPattern,
	OscErrorSourceEndsBeforeStartOfTypeTagString,
	OscErrorTypeTagStringToLong,
	OscErrorSourceEndsBeforeEndOfTypeTagString,
	OscErrorUnexpectedEndOfSource,
	OscErrorNoArgumentsAvailable,
	OscErrorUnexpectedArgumentType,
	OscErrorMessageTooShortForArgumentType,

	/* OscBundle errors  */
	OscErrorBundleFull,
	OscErrorBundleSizeTooSmall,
	OscErrorBundleSizeTooLarge,
	OscErrorNoHashAtStartOfBundle,
	OscErrorBundleElementNotAvailable,
	OscErrorNegativeBundleElementSize,
	OscErrorInvalidElementSize,

	/* OscPacket errors  */
	OscErrorInvalidContents,
	OscErrorPacketSizeTooLarge,
	OscErrorContentsEmpty,

	/* OscSlip errors  */
	OscErrorEncodedSlipPacketTooLong,
	OscErrorUnexpectedByteAfterSlipEsc,
	OscErrorDecodedSlipPacketTooLong,
} OscError;

char* OscErrorGetMessage(const OscError oscError);

#endif
