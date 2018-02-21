using NUnit.Framework;

namespace sdproject
{
	internal class MathTests
	{
		[Test, TestCase(5, 2, 10), TestCase(5, 3, 10)]
		public void MinorCountTest(int matrixSize, int minorSize, int expectedResult)
		{
			int result = MathUtils.MaxMinorAmount(matrixSize, minorSize);
			Assert.AreEqual(expectedResult, result);
		}
	}
}
