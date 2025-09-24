using System;

namespace Project_H_ECS.ECS.Helper
{
	public static class MathUtil
	{
		public static int GetPowerOfTwo(int number)
		{
			if (number <= 0 || (number & (number - 1)) != 0)
				throw new ArgumentException("Number must be a positive power of 2.", nameof(number));

			int log = 0;
			if ((number & 0xFFFF0000) != 0)
			{
				number >>= 16;
				log += 16;
			}

			if ((number & 0xFF00) != 0)
			{
				number >>= 8;
				log += 8;
			}

			if ((number & 0xF0) != 0)
			{
				number >>= 4;
				log += 4;
			}

			if ((number & 0xC) != 0)
			{
				number >>= 2;
				log += 2;
			}

			if ((number & 0x2) != 0)
			{
				log += 1;
			}

			return log;
		}

		public static int TrailingZeroCount(ulong value)
		{
			if (value == 0) return 64;

			int count = 0;

			// Binary search approach (log2 steps instead of looping all 64 bits)
			if ((value & 0xFFFFFFFF) == 0)
			{
				count += 32;
				value >>= 32;
			}

			if ((value & 0xFFFF) == 0)
			{
				count += 16;
				value >>= 16;
			}

			if ((value & 0xFF) == 0)
			{
				count += 8;
				value >>= 8;
			}

			if ((value & 0xF) == 0)
			{
				count += 4;
				value >>= 4;
			}

			if ((value & 0x3) == 0)
			{
				count += 2;
				value >>= 2;
			}

			if ((value & 0x1) == 0)
			{
				count += 1;
			}

			return count;
		}
	}
}