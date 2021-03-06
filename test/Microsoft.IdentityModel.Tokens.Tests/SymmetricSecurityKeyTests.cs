//------------------------------------------------------------------------------
//
// Copyright (c) Microsoft Corporation.
// All rights reserved.
//
// This code is licensed under the MIT License.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files(the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and / or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions :
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//
//------------------------------------------------------------------------------

using System;
using Microsoft.IdentityModel.TestUtils;
using Xunit;

using ALG = Microsoft.IdentityModel.Tokens.SecurityAlgorithms;
using EE = Microsoft.IdentityModel.TestUtils.ExpectedException;
using KEY = Microsoft.IdentityModel.TestUtils.KeyingMaterial;

#pragma warning disable CS3016 // Arrays as attribute arguments is not CLS-compliant

namespace Microsoft.IdentityModel.Tokens.Tests
{
    public class SymmetricSecurityKeyTests
    {

        [Theory, MemberData(nameof(ConstructorDataSet))]
        public void Constructor(byte[] key, EE ee)
        {
            try
            {
                var symmetricSecurityKey = new SymmetricSecurityKey(key);
                ee.ProcessNoException();
            }
            catch (Exception exception)
            {
                ee.ProcessException(exception);
            }
        }

        public static TheoryData<byte[], EE> ConstructorDataSet
        {
            get
            {
                var dataset = new TheoryData<byte[], EE>();
                dataset.Add(KEY.DefaultSymmetricKeyBytes_256, EE.NoExceptionExpected);
                dataset.Add(null, EE.ArgumentNullException());
                dataset.Add(new byte[0], EE.ArgumentException());
                return dataset;
            }
        }

        [Theory, MemberData(nameof(IsSupportedAlgDataSet))]
        public void IsSupportedAlgorithm(SymmetricSecurityKey key, string alg, bool expectedResult)
        {
            if (key.CryptoProviderFactory.IsSupportedAlgorithm(alg, key) != expectedResult)
                Assert.True(false, string.Format("{0} failed with alg: {1}. ExpectedResult: {2}", key, alg, expectedResult));
        }

        public static TheoryData<SymmetricSecurityKey, string, bool> IsSupportedAlgDataSet
        {
            get
            {
                var dataset = new TheoryData<SymmetricSecurityKey, string, bool>();
                dataset.Add(KEY.DefaultSymmetricSecurityKey_256, ALG.HmacSha256, true);
                dataset.Add(KEY.SymmetricSecurityKey2_256, ALG.HmacSha384Signature, true);
                dataset.Add(KEY.DefaultSymmetricSecurityKey_256, ALG.Aes128Encryption, false);

                SymmetricSecurityKey testKey = new SymmetricSecurityKey(KEY.DefaultSymmetricKeyBytes_256);
                testKey.CryptoProviderFactory = new CustomCryptoProviderFactory(new string[] { ALG.Aes128Encryption });
                dataset.Add(testKey, ALG.Aes128Encryption, true);
                return dataset;
            }
        }
    }
}

#pragma warning restore CS3016 // Arrays as attribute arguments is not CLS-compliant
