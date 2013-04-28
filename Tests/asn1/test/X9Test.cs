using System;
using System.IO;

using NUnit.Framework;

using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.Sec;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Math.EC;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.Utilities.Test;

namespace Org.BouncyCastle.Asn1.Tests
{
	[TestFixture]
	public class X9Test
		: SimpleTest
	{
		private static readonly byte[] namedPub = Base64.Decode("MBowEwYHKoZIzj0CAQYIKoZIzj0DAQEDAwADAQ==");
		private static readonly byte[] expPub = Base64.Decode(
			  "MIHfMIHXBgcqhkjOPQIBMIHLAgEBMCkGByqGSM49AQECHn///////////////3///////4AAAA"
			+ "AAAH///////zBXBB5///////////////9///////+AAAAAAAB///////wEHiVXBfoqMGZUsfTL"
			+ "A9anUKMMJQEC1JiHF9m6FattPgMVAH1zdBaP/jRxtgqFdoahlHXTv6L/BB8DZ2iujhi7ks/PAF"
			+ "yUmqLG2UhT0OZgu/hUsclQX+laAh5///////////////9///+XXetBs6YFfDxDIUZSZVECAQED"
			+ "AwADAQ==");

		private static readonly byte[] namedPriv = Base64.Decode("MCICAQAwEwYHKoZIzj0CAQYIKoZIzj0DAQEECDAGAgEBBAEK");
		private static readonly byte[] expPriv = Base64.Decode(
			  "MIHnAgEAMIHXBgcqhkjOPQIBMIHLAgEBMCkGByqGSM49AQECHn///////////////3///////4"
			+ "AAAAAAAH///////zBXBB5///////////////9///////+AAAAAAAB///////wEHiVXBfoqMGZU"
			+ "sfTLA9anUKMMJQEC1JiHF9m6FattPgMVAH1zdBaP/jRxtgqFdoahlHXTv6L/BB8DZ2iujhi7ks"
			+ "/PAFyUmqLG2UhT0OZgu/hUsclQX+laAh5///////////////9///+XXetBs6YFfDxDIUZSZVEC"
			+ "AQEECDAGAgEBBAEU");

		private void EncodePublicKey()
		{
			X9ECParameters ecP = X962NamedCurves.GetByOid(X9ObjectIdentifiers.Prime239v3);

			if (X9IntegerConverter.GetByteLength(ecP.Curve) != 30)
			{
				Fail("wrong byte length reported for curve");
			}

			if (ecP.Curve.FieldSize != 239)
			{
				Fail("wrong field size reported for curve");
			}

			//
			// named curve
			//
			X962Parameters _params = new X962Parameters(X9ObjectIdentifiers.Prime192v1);

			X9ECPoint pPoint = new X9ECPoint(
				new FPPoint(ecP.Curve, new FPFieldElement(BigInteger.Two, BigInteger.One),
				new FPFieldElement(BigInteger.ValueOf(4), BigInteger.ValueOf(3)),
				true));

			Asn1OctetString p = (Asn1OctetString) pPoint.ToAsn1Object();

			if (p == null)
			{
				Fail("failed to convert to ASN.1");
			}

			SubjectPublicKeyInfo info = new SubjectPublicKeyInfo(new AlgorithmIdentifier(X9ObjectIdentifiers.IdECPublicKey, _params), p.GetOctets());

			if (!Arrays.AreEqual(info.GetEncoded(), namedPub))
			{
				Fail("failed public named generation");
			}

			Asn1Object o = Asn1Object.FromByteArray(namedPub);

			if (!info.Equals(o))
			{
				Fail("failed public named equality");
			}

			//
			// explicit curve parameters
			//
			_params = new X962Parameters(ecP);

			info = new SubjectPublicKeyInfo(new AlgorithmIdentifier(X9ObjectIdentifiers.IdECPublicKey, _params), p.GetOctets());

			if (!Arrays.AreEqual(info.GetEncoded(), expPub))
			{
				Fail("failed public explicit generation");
			}

			o = Asn1Object.FromByteArray(expPub);

			if (!info.Equals(o))
			{
				Fail("failed public explicit equality");
			}
		}

		private void EncodePrivateKey()
		{
			X9ECParameters ecP = X962NamedCurves.GetByOid(X9ObjectIdentifiers.Prime239v3);

			//
			// named curve
			//
			X962Parameters _params = new X962Parameters(X9ObjectIdentifiers.Prime192v1);

			X9ECPoint pPoint = new X9ECPoint(
				new FPPoint(ecP.Curve, new FPFieldElement(BigInteger.Two, BigInteger.One),
				new FPFieldElement(BigInteger.ValueOf(4), BigInteger.ValueOf(3)),
				true));

			Asn1OctetString p = (Asn1OctetString) pPoint.ToAsn1Object();

			if (p == null)
				Fail("failed to convert to ASN.1");

			PrivateKeyInfo info = new PrivateKeyInfo(new AlgorithmIdentifier(X9ObjectIdentifiers.IdECPublicKey, _params), new ECPrivateKeyStructure(BigInteger.Ten).ToAsn1Object());

			if (!Arrays.AreEqual(info.GetEncoded(), namedPriv))
			{
				Fail("failed private named generation");
			}

			Asn1Object o = Asn1Object.FromByteArray(namedPriv);

			if (!info.Equals(o))
			{
				Fail("failed private named equality");
			}

			//
			// explicit curve parameters
			//
			_params = new X962Parameters(ecP);

			info = new PrivateKeyInfo(new AlgorithmIdentifier(X9ObjectIdentifiers.IdECPublicKey, _params), new ECPrivateKeyStructure(BigInteger.ValueOf(20)).ToAsn1Object());

			if (!Arrays.AreEqual(info.GetEncoded(), expPriv))
			{
				Fail("failed private explicit generation");
			}

			o = Asn1Object.FromByteArray(expPriv);

			if (!info.Equals(o))
			{
				Fail("failed private explicit equality");
			}
		}

		public override void PerformTest()
		{
			EncodePublicKey();
			EncodePrivateKey();
		}

		public override string Name
		{
			get { return "X9"; }
		}

		public static void Main(
			string[] args)
		{
			RunTest(new X9Test());
		}

		[Test]
		public void TestFunction()
		{
			string resultText = Perform().ToString();

			Assert.AreEqual(Name + ": Okay", resultText);
		}
	}
}
