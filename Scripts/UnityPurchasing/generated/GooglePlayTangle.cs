// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("BlBqkO4CDZmvfyWWu5/STJAAvBBTKXMzNHMzuCLP+I0H/KGY9aqKbUiG4rP+HdUh/HwshPDgoRl9+MHckZ+4upTDGzMgSLGluXfNp3hPbS2PYSOekXtvk/0eito7wtC/bzY3VEqhM6riMgQjcDojW2pzLDuee/iP8ev97Ut6j47pe2DDa/ybrgGHiBmyAst5cQ4yf7aEgpl+YsntrIGfd6UXlLelmJOcvxPdE2KYlJSUkJWWF5SalaUXlJ+XF5SUlTKWDYXhzWSHufezrg9kEa/BVLYF9SFW03UFjleDyjrVc62264OVi9HJJMaBgf4cOYrK5oxqPJKGrccEwbaDqtw5r3U2ExtGI8PMdbGBW9lYyRCaD8JXytbGgufSASMuqJeWlJWU");
        private static int[] order = new int[] { 5,3,9,12,9,13,9,10,13,12,13,11,13,13,14 };
        private static int key = 149;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
