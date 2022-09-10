﻿using XenOS.Libraries.Graphics;
using Cosmos.Core;

namespace XenOS.Libraries.Resource.Images
{
    public static class TGA
    {
        public static FrameBuffer FromTGA(byte[] Binary)
        {
            uint i, j, k, x, y, w = (uint)((Binary[13] << 8) + Binary[12]), h = (uint)((Binary[15] << 8) + Binary[14]), o = (uint)((Binary[11] << 8) + Binary[10]);
            uint m = (uint)((Binary[1] != 0 ? (Binary[7] >> 3) * Binary[5] : 0) + 18);

            if (w < 1 || h < 1)
            {
                return null;
            }

            uint[] Data = new uint[(w * h + 2) * 4];

            if (Data.Length == 0)
                return null;

            switch (Binary[2])
            {
                case 1:
                    if (Binary[6] != 0 || Binary[4] != 0 || Binary[3] != 0 || (Binary[7] != 24 && Binary[7] != 32))
                    {
                        GCImplementation.Free(Data);
                        return null;
                    }
                    for (y = i = 0; y < h; y++)
                    {
                        k = ((o != 0 ? h - y - 1 : y) * w);
                        for (x = 0; x < w; x++)
                        {
                            j = (uint)(Binary[m + k++] * (Binary[7] >> 3) + 18);
                            Data[2 + i++] = (uint)(((Binary[7] == 32 ? Binary[j + 3] : 0xFF) << 24) | (Binary[j + 2] << 16) | (Binary[j + 1] << 8) | Binary[j]);
                        }
                    }
                    break;
                case 2:
                    if (Binary[5] != 0 || Binary[6] != 0 || Binary[1] != 0 || (Binary[16] != 24 && Binary[16] != 32))
                    {
                        GCImplementation.Free(Data);
                        return null;
                    }
                    for (y = i = 0; y < h; y++)
                    {
                        j = ((uint)((o != 0 ? h - y - 1 : y) * w * (Binary[16] >> 3)));
                        for (x = 0; x < w; x++)
                        {
                            Data[2 + i++] = (uint)(((Binary[16] == 32 ? Binary[j + 3] : 0xFF) << 24) | (Binary[j + 2] << 16) | (Binary[j + 1] << 8) | Binary[j]);
                            j += (uint)(Binary[16] >> 3);
                        }
                    }
                    break;
                case 9:
                    if (Binary[6] != 0 || Binary[4] != 0 || Binary[3] != 0 || (Binary[7] != 24 && Binary[7] != 32))
                    {
                        GCImplementation.Free(Data);
                        return null;
                    }
                    y = i = 0;
                    for (x = 0; x < w * h && m < Binary.Length;)
                    {
                        k = Binary[m++];
                        if (k > 127)
                        {
                            k -= 127; x += k;
                            j = (uint)(Binary[m++] * (Binary[7] >> 3) + 18);
                            while (k-- != 0)
                            {
                                if ((i % w) != 0) { i = ((o != 0 ? h - y - 1 : y) * w); y++; }
                                Data[2 + i++] = (uint)(((Binary[7] == 32 ? Binary[j + 3] : 0xFF) << 24) | (Binary[j + 2] << 16) | (Binary[j + 1] << 8) | Binary[j]);
                            }
                        }
                        else
                        {
                            k++; x += k;
                            while (k-- != 0)
                            {
                                j = (uint)(Binary[m++] * (Binary[7] >> 3) + 18);
                                if ((i % w) != 0) { i = ((o != 0 ? h - y - 1 : y) * w); y++; }
                                Data[2 + i++] = (uint)(((Binary[7] == 32 ? Binary[j + 3] : 0xFF) << 24) | (Binary[j + 2] << 16) | (Binary[j + 1] << 8) | Binary[j]);
                            }
                        }
                    }
                    break;
                case 10:
                    if (Binary[5] != 0 || Binary[6] != 0 || Binary[1] != 0 || (Binary[16] != 24 && Binary[16] != 32))
                    {
                        GCImplementation.Free(Data);
                        return null;
                    }
                    y = i = 0;
                    for (x = 0; x < w * h && m < Binary.Length;)
                    {
                        k = Binary[m++];
                        if (k > 127)
                        {
                            k -= 127; x += k;
                            while (k-- != 0)
                            {
                                if ((i % w) != 0) { i = ((o != 0 ? h - y - 1 : y) * w); y++; }
                                Data[2 + i++] = (uint)(((Binary[16] == 32 ? Binary[m + 3] : 0xFF) << 24) | (Binary[m + 2] << 16) | (Binary[m + 1] << 8) | Binary[m]);
                            }
                            m += (uint)Binary[16] >> 3;
                        }
                        else
                        {
                            k++; x += k;
                            while (k-- != 0)
                            {
                                if ((i % w) != 0) { i = ((o != 0 ? h - y - 1 : y) * w); y++; }
                                Data[2 + i++] = (uint)(((Binary[16] == 32 ? Binary[m + 3] : 0xFF) << 24) | (Binary[m + 2] << 16) | (Binary[m + 1] << 8) | Binary[m]);
                                m += (uint)(Binary[16] >> 3);
                            }
                        }
                    }
                    break;
                default:
                    GCImplementation.Free(Data);
                    return null;
            }
            Data[0] = w;
            Data[1] = h;

            FrameBuffer TMP = new(Data[0], Data[1]);
            for (int I = 0; I < TMP.Size; I++)
            {
                TMP[I] = Color.FromARGB(Data[I + 2]);
            }
            return TMP;
        }
    }
}