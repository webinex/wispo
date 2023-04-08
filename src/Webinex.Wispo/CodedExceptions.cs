using Webinex.Coded;

namespace Webinex.Wispo
{
    internal static class CodedExceptions
    {
        public static CodedException AnotherRecipient()
        {
            throw new CodedException(Code.FORBIDDEN + ".WSP.ANTHRREC",
                defaultMessage: "Attempt to access notification which belongs to another recipient");
        }
    }
}