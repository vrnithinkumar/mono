//
// System.Security.Cryptography SymmetricAlgorithm Class implementation
//
// Authors:
//   Thomas Neidhart (tome@sbox.tugraz.at)
//

using System;
using System.Security.Cryptography;

namespace System.Security.Cryptography {
	public abstract class SymmetricAlgorithm {
		protected int BlockSizeValue; // The block size of the cryptographic operation in bits. 
		protected int FeedbackSizeValue; // The feedback size of the cryptographic operation in bits. 
		protected byte[] IVValue; // The initialization vector ( IV) for the symmetric algorithm. 
		protected int KeySizeValue; // The size of the secret key used by the symmetric algorithm in bits. 
		protected byte[] KeyValue; // The secret key for the symmetric algorithm. 
		protected KeySizes[] LegalBlockSizesValue; // Specifies the block sizes that are supported by the symmetric algorithm. 
		protected KeySizes[] LegalKeySizesValue; // Specifies the key sizes that are supported by the symmetric algorithm. 
		protected CipherMode ModeValue; // Represents the cipher mode used in the symmetric algorithm. 
		protected PaddingMode PaddingValue; // Represents the padding mode used in the symmetric algorithm. 

		/// <summary>
		/// Called from constructor of derived class.
		/// </summary>
		public SymmetricAlgorithm () {
		  throw new CryptographicException();
		}
		
		/// <summary>
		/// Called from constructor of derived class.
		/// </summary>
		~SymmetricAlgorithm () {
			if (KeyValue != null) {
				Array.Clear(KeyValue, 0, KeyValue.Length);
				KeyValue = null;
			}
		}

		/// <summary>
		/// Gets or sets the actual BlockSize
		/// </summary>
		public virtual int BlockSize {
			get {
				return this.BlockSizeValue;
			}
			set {
				if (IsLegalKeySize(this.LegalBlockSizesValue, value))
					this.BlockSizeValue = value;
				else
					throw new CryptographicException("block size not supported by algorithm");
			}
		}

		/// <summary>
		/// Gets or sets the actual FeedbackSize
		/// </summary>
		public virtual int FeedbackSize {
			get {
				return this.FeedbackSizeValue;
			}
			set {
				if (value > this.BlockSizeValue)
					throw new CryptographicException("feedback size larger than block size");
				else
					this.FeedbackSizeValue = value;
			}
		}
		
		/// <summary>
		/// Gets or sets the actual Initial Vector
		/// </summary>
		public virtual byte[] IV {
			get {
				if (this.IVValue == null)
					GenerateIV();

				return this.IVValue;
			}
			set {
				if (value == null)
					throw new ArgumentNullException("tried setting initial vector to null");
					
				// FIXME: dont know if to compare with block or key size	
				if (value.Length != this.KeySizeValue)
					throw new CryptographicException("tried setting initial vector with illegal size");
				
				this.IVValue = new byte [value.Length];
				System.Array.Copy (value, 0, this.IVValue, 0, value.Length);
			}
		}

		/// <summary>
		/// Gets or sets the actual key
		/// </summary>
		public virtual byte[] Key {
			get {
				if (this.KeyValue == null)
					GenerateKey();

				return this.KeyValue;
			}
			set {
				if (value == null)
					throw new ArgumentNullException("tried setting key to null");

				if (!IsLegalKeySize(this.LegalKeySizesValue, value.Length))
					throw new CryptographicException("key size not supported by algorithm");

				this.KeySizeValue = value.Length;
				this.KeyValue = new byte [this.KeySizeValue];
				System.Array.Copy (value, 0, this.KeyValue, 0, this.KeySizeValue);
			}
		}
		
		/// <summary>
		/// Gets or sets the actual key size
		/// </summary>
		public virtual int KeySize {
			get {
				return this.KeySizeValue;
			}
			set {
				if (!IsLegalKeySize(this.LegalKeySizesValue, value))
					throw new CryptographicException("key size not supported by algorithm");
				
				this.KeyValue = null;
				this.KeySizeValue = value;
			}
		}

		/// <summary>
		/// Gets all legal block sizes
		/// </summary>
		public virtual KeySizes[] LegalBlockSizes {
			get {
				return this.LegalBlockSizesValue;
			}
		}

		/// <summary>
		/// Gets all legal key sizes
		/// </summary>
		public virtual KeySizes[] LegalKeySizes {
			get {
				return this.LegalKeySizesValue;
			}
		}

		/// <summary>
		/// Gets or sets the actual cipher mode
		/// </summary>
		public virtual CipherMode Mode {
			get {
				return this.ModeValue;
			}
			set {
				if (Enum.IsDefined(ModeValue.GetType(), value))
					this.ModeValue = value;
				else
					throw new CryptographicException("padding mode not available");
			}
		}

		/// <summary>
		/// Gets or sets the actual padding
		/// </summary>
		public virtual PaddingMode Padding {
			get {
				return this.PaddingValue;
			}
			set {
				if (Enum.IsDefined(PaddingValue.GetType(), value))
					this.PaddingValue = value;
				else
					throw new CryptographicException("padding mode not available");
			}
		}

		/// <summary>
		/// LAMESPEC: no idea what this does
		/// </summary>
		public virtual ICryptoTransform CreateDecryptor() {
			// TODO: implement
			return null;
		}

		/// <summary>
		/// LAMESPEC: no idea what this does
		/// </summary>
		public abstract ICryptoTransform CreateDecryptor(byte[] rgbKey, byte[] rgbIV);

		/// <summary>
		/// LAMESPEC: no idea what this does
		/// </summary>
		public virtual ICryptoTransform CreateEncryptor() {
			// TODO: implement
			return null;
		}

		/// <summary>
		/// LAMESPEC: no idea what this does
		/// </summary>
		public abstract ICryptoTransform CreateEncryptor(byte[] rgbKey, byte[] rgbIV);

		/// <summary>
		/// used to generate an inital vector if none is specified
		/// </summary>
		public abstract void GenerateIV();

		/// </summary>
		/// used to generate a random key if none is specified
		/// </summary>
		public abstract void GenerateKey();

		internal bool IsLegalKeySize(KeySizes[] LegalKeys, int Size) {
			foreach (KeySizes LegalKeySize in LegalKeys) {
				for (int i=LegalKeySize.MinSize; i<=LegalKeySize.MaxSize; i+=LegalKeySize.SkipSize) {
					if (i == Size)
						return true;
				}
			}
			return false;
		}
		
		/// <summary>
		/// Checks wether the given keyLength is valid for the current algorithm
		/// </summary>
		/// <param name="bitLength">the given keyLength</param>
		public bool ValidKeySize(int bitLength) {
			return IsLegalKeySize(LegalKeySizesValue, bitLength);
		}
		
		/// <summary>
		/// Creates the default implementation of the default symmetric algorithm (RC2).
		/// </summary>
		public static SymmetricAlgorithm Create () {
			// TODO: default algorithm is RijndaelManaged
			return null;
		}
	
		/// <summary>
		/// Creates a specific implementation of the given symmetric algorithm.
		/// </summary>
		/// <param name="algName">the given algorithm</param>
		public static SymmetricAlgorithm Create (string algName) {
			// TODO: use reflection to create a new instance of the given algorithm
			return null;
		}
	}
}

