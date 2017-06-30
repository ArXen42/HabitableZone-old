using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine.Assertions.Comparers;

namespace HabitableZone.Common
{
	public class Assert
	{
		[Conditional(CompilationConstant)]
		public static void IsNull<T>(T value) where T : class
		{
			IsNull(value, null);
		}

		[Conditional(CompilationConstant)]
		public static void IsNull<T>(T value, String message) where T : class
		{
			if (value != null)
				Fail(AssertionMessageUtil.NullFailureMessage(value, true), message);
		}

		[Conditional(CompilationConstant)]
		public static void IsNotNull<T>(T value) where T : class
		{
			IsNotNull(value, null);
		}

		[Conditional(CompilationConstant)]
		public static void IsNotNull<T>(T value, String message) where T : class
		{
			if (value == null)
				Fail(AssertionMessageUtil.NullFailureMessage(value, false), message);
		}


		[Conditional(CompilationConstant)]
		public static void AreApproximatelyEqual(Single expected, Single actual)
		{
			AreEqual(expected, actual, null, FloatComparer.s_ComparerWithDefaultTolerance);
		}

		[Conditional(CompilationConstant)]
		public static void AreApproximatelyEqual(Single expected, Single actual, String message)
		{
			AreEqual(expected, actual, message, FloatComparer.s_ComparerWithDefaultTolerance);
		}

		[Conditional(CompilationConstant)]
		public static void AreApproximatelyEqual(Single expected, Single actual, Single tolerance)
		{
			AreApproximatelyEqual(expected, actual, tolerance, null);
		}

		[Conditional(CompilationConstant)]
		public static void AreApproximatelyEqual(Single expected, Single actual, Single tolerance, String message)
		{
			AreEqual(expected, actual, message, new FloatComparer(tolerance));
		}

		[Conditional(CompilationConstant)]
		public static void AreNotApproximatelyEqual(Single expected, Single actual)
		{
			AreNotEqual(expected, actual, null, FloatComparer.s_ComparerWithDefaultTolerance);
		}

		[Conditional(CompilationConstant)]
		public static void AreNotApproximatelyEqual(Single expected, Single actual, String message)
		{
			AreNotEqual(expected, actual, message, FloatComparer.s_ComparerWithDefaultTolerance);
		}

		[Conditional(CompilationConstant)]
		public static void AreNotApproximatelyEqual(Single expected, Single actual, Single tolerance)
		{
			AreNotApproximatelyEqual(expected, actual, tolerance, null);
		}

		[Conditional(CompilationConstant)]
		public static void AreNotApproximatelyEqual(Single expected, Single actual, Single tolerance, String message)
		{
			AreNotEqual(expected, actual, message, new FloatComparer(tolerance));
		}


		[Conditional(CompilationConstant)]
		public static void IsTrue(Boolean condition)
		{
			IsTrue(condition, null);
		}

		[Conditional(CompilationConstant)]
		public static void IsTrue(Boolean condition, String message)
		{
			if (!condition)
				Fail(AssertionMessageUtil.BooleanFailureMessage(true), message);
		}

		[Conditional(CompilationConstant)]
		public static void IsFalse(Boolean condition)
		{
			IsFalse(condition, null);
		}

		[Conditional(CompilationConstant)]
		public static void IsFalse(Boolean condition, String message)
		{
			if (condition)
				Fail(AssertionMessageUtil.BooleanFailureMessage(false), message);
		}

		[Conditional(CompilationConstant)]
		public static void AreEqual<T>(T expected, T actual)
		{
			AreEqual(expected, actual, null);
		}

		[Conditional(CompilationConstant)]
		public static void AreEqual<T>(T expected, T actual, String message)
		{
			AreEqual(expected, actual, message, EqualityComparer<T>.Default);
		}

		[Conditional(CompilationConstant)]
		public static void AreEqual<T>(T expected, T actual, String message, IEqualityComparer<T> comparer)
		{
			if (!comparer.Equals(actual, expected))
				Fail(AssertionMessageUtil.GetEqualityMessage(actual, expected, true), message);
		}

		[Conditional(CompilationConstant)]
		public static void AreNotEqual<T>(T expected, T actual)
		{
			AreNotEqual(expected, actual, null);
		}

		[Conditional(CompilationConstant)]
		public static void AreNotEqual<T>(T expected, T actual, String message)
		{
			AreNotEqual(expected, actual, message, EqualityComparer<T>.Default);
		}

		[Conditional(CompilationConstant)]
		public static void AreNotEqual<T>(T expected, T actual, String message, IEqualityComparer<T> comparer)
		{
			if (comparer.Equals(actual, expected))
				Fail(AssertionMessageUtil.GetEqualityMessage(actual, expected, false), message);
		}

		private static void Fail(String message, String userMessage)
		{
			if (message == null)
				message = "Assertion has failed\n";

			throw new AssertionException(message, userMessage);
		}

		private const String CompilationConstant = "ASSERTIONS";
	}

	internal class AssertionMessageUtil
	{
		public static String GetMessage(String failureMessage)
		{
			return $"Assertion failed. {failureMessage}";
		}

		public static String GetMessage(String failureMessage, String expected)
		{
			return GetMessage($"{failureMessage}{Environment.NewLine}Expected: {expected}");
		}

		public static String GetEqualityMessage(Object actual, Object expected, Boolean expectEqual)
		{
			return GetMessage($"Values are {(!expectEqual ? "" : "not ")}equal.",
				String.Format("{0} {2} {1}", actual, expected, !expectEqual ? "!=" : "=="));
		}

		public static String NullFailureMessage(Object value, Boolean expectNull)
		{
			return GetMessage($"Value was {(!expectNull ? "" : "not ")}Null",
				$"Value was {(!expectNull ? "not " : "")}Null");
		}

		public static String BooleanFailureMessage(Boolean expected)
		{
			return GetMessage("Value was " + !expected, expected.ToString());
		}
	}

	public class AssertionException : Exception
	{
		public AssertionException(String message, String userMessage) : base(message)
		{
			_userMessage = userMessage;
		}

		public override String Message
		{
			get
			{
				String str = base.Message;
				if (_userMessage != null)
					str = $"{str}\n{_userMessage}";
				return str;
			}
		}

		private readonly String _userMessage;
	}
}