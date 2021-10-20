﻿using System;

namespace Service.Helpers
{
    public class Validator
    {
        public static bool VerifyGuidType(string id, out Guid guid)
        {
			guid = Guid.Empty;
			try
			{
				guid = new Guid(id);
				return true;
			}
			catch
			{
				return false;
			}
		}
    }
}
