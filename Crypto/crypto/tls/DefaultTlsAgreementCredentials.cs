using System;

using Org.BouncyCastle.Crypto.Agreement;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Crypto.Tls
{
	public class DefaultTlsAgreementCredentials
		: TlsAgreementCredentials
	{
		protected Certificate clientCert;
		protected IAsymmetricKeyParameter clientPrivateKey;

		protected IBasicAgreement basicAgreement;

		public DefaultTlsAgreementCredentials(Certificate clientCertificate, IAsymmetricKeyParameter clientPrivateKey)
		{
			if (clientCertificate == null)
			{
				throw new ArgumentNullException("clientCertificate");
			}
			if (clientCertificate.certs.Length == 0)
			{
				throw new ArgumentException("cannot be empty", "clientCertificate");
			}
			if (clientPrivateKey == null)
			{
				throw new ArgumentNullException("clientPrivateKey");
			}
			if (!clientPrivateKey.IsPrivate)
			{
				throw new ArgumentException("must be private", "clientPrivateKey");
			}

			if (clientPrivateKey is DHPrivateKeyParameters)
			{
				basicAgreement = new DHBasicAgreement();
			}
			else if (clientPrivateKey is ECPrivateKeyParameters)
			{
				basicAgreement = new EcdhBasicAgreement();
			}
			else
			{
				throw new ArgumentException("type not supported: "
					+ clientPrivateKey.GetType().FullName, "clientPrivateKey");
			}

			this.clientCert = clientCertificate;
			this.clientPrivateKey = clientPrivateKey;
		}

		public virtual Certificate Certificate
		{
			get { return clientCert; }
		}

		public virtual byte[] GenerateAgreement(IAsymmetricKeyParameter serverPublicKey)
		{
			basicAgreement.Init(clientPrivateKey);
			IBigInteger agreementValue = basicAgreement.CalculateAgreement(serverPublicKey);
			return BigIntegers.AsUnsignedByteArray(agreementValue);
		}
	}
}
