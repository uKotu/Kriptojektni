using System;

namespace Kriptojektni.CryptoLogic
{
    [Serializable]
    class EncryptedFile
    {
        private byte[] encryptedDataWithDigitalSignature;
        private byte[] key;
        private byte[] IV;

        private byte[] sender;

        public EncryptedFile(byte[] dataAndDigitalSignature, byte[] key, byte[] iv)
        {
            encryptedDataWithDigitalSignature = dataAndDigitalSignature; this.key = key; IV = iv;
        }
        public void setSender(byte[] senderInput)
        {
            sender = senderInput;
        }
        public byte[] getSender()
        {
            return sender;
        }
        public byte[] getData()
        {
            return encryptedDataWithDigitalSignature;
        }
        public byte[] getKey()
        {
            return key;
        }
        public byte[] getIV()
        {
            return IV;
        }
    }
}
