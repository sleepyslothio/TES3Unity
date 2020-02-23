using System;
using System.IO;
using System.Text;
using UnityEngine;

/// <summary>
/// An improved BinaryReader for Unity.
/// </summary>
public class UnityBinaryReader : IDisposable
{
    private BinaryReader _reader;
    // A buffer for read bytes the size of a decimal variable. Created to minimize allocations. 
    private byte[] _readBuffer = new byte[16];

    public Stream BaseStream => _reader.BaseStream;

    public UnityBinaryReader(Stream input)
    {
        _reader = new BinaryReader(input, Encoding.ASCII);
    }

    public UnityBinaryReader(Stream input, Encoding encoding)
    {
        _reader = new BinaryReader(input, encoding);
    }

    void IDisposable.Dispose()
    {
        Close();
    }

    ~UnityBinaryReader()
    {
        Close();
    }

    public void Close()
    {
        if (_reader != null)
        {
            _reader.Close();
            _reader = null;
        }
    }

    public byte ReadByte()
    {
        return _reader.ReadByte();
    }

    public sbyte ReadSByte()
    {
        return _reader.ReadSByte();
    }

    public void Read(byte[] buffer, int index, int count)
    {
        _reader.Read(buffer, index, count);
    }

    public byte[] ReadBytes(int count)
    {
        return _reader.ReadBytes(count);
    }

    public byte[] ReadRestOfBytes()
    {
        var remainingByteCount = _reader.BaseStream.Length - _reader.BaseStream.Position;

        Debug.Assert(remainingByteCount <= int.MaxValue);

        return _reader.ReadBytes((int)remainingByteCount);
    }

    public void ReadRestOfBytes(byte[] buffer, int startIndex)
    {
        var remainingByteCount = _reader.BaseStream.Length - _reader.BaseStream.Position;

        Debug.Assert((startIndex >= 0) && (remainingByteCount <= int.MaxValue) && ((startIndex + remainingByteCount) <= buffer.Length));

        _reader.Read(buffer, startIndex, (int)remainingByteCount);
    }

    public string ReadASCIIString(int length)
    {
        Debug.Assert(length >= 0);

        return Encoding.ASCII.GetString(_reader.ReadBytes(length));
    }

    public string ReadPossiblyNullTerminatedASCIIString(int lengthIncludingPossibleNullTerminator)
    {
        Debug.Assert(lengthIncludingPossibleNullTerminator > 0);

        var bytes = _reader.ReadBytes(lengthIncludingPossibleNullTerminator);

        // Ignore the null terminator.
        var charCount = (ArrayUtils.Last(bytes) != 0) ? bytes.Length : (bytes.Length - 1);
        return Encoding.Default.GetString(bytes, 0, charCount);
    }

    #region Little Endian

    public bool ReadLEBool32()
    {
        return ReadLEUInt32() != 0;
    }

    public ushort ReadLEUInt16()
    {
        _reader.Read(_readBuffer, 0, 2);

        return (ushort)((_readBuffer[1] << 8) | _readBuffer[0]);
    }
    public uint ReadLEUInt32()
    {
        _reader.Read(_readBuffer, 0, 4);

        return ((uint)_readBuffer[3] << 24) | ((uint)_readBuffer[2] << 16) | ((uint)_readBuffer[1] << 8) | _readBuffer[0];
    }
    public ulong ReadLEUInt64()
    {
        _reader.Read(_readBuffer, 0, 8);

        return ((ulong)_readBuffer[7] << 56) | ((ulong)_readBuffer[6] << 48) | ((ulong)_readBuffer[5] << 40) | ((ulong)_readBuffer[4] << 32) | ((ulong)_readBuffer[3] << 24) | ((ulong)_readBuffer[2] << 16) | ((ulong)_readBuffer[1] << 8) | _readBuffer[0];
    }

    public short ReadLEInt16()
    {
        _reader.Read(_readBuffer, 0, 2);

        return BitConverter.ToInt16(_readBuffer, 0);
    }

    public int ReadLEInt32()
    {
        _reader.Read(_readBuffer, 0, 4);

        return BitConverter.ToInt32(_readBuffer, 0);
    }

    public long ReadLEInt64()
    {
        _reader.Read(_readBuffer, 0, 8);

        return BitConverter.ToInt32(_readBuffer, 0);
    }

    public float ReadLESingle()
    {
        _reader.Read(_readBuffer, 0, 4);

        return BitConverter.ToSingle(_readBuffer, 0);
    }

    public double ReadLEDouble()
    {
        _reader.Read(_readBuffer, 0, 8);

        return BitConverter.ToDouble(_readBuffer, 0);
    }

    public byte[] ReadLELength32PrefixedBytes()
    {
        var length = ReadLEInt32();
        var count = (int)length;
        return _reader.ReadBytes(count);
    }

    public string ReadLELength32PrefixedASCIIString()
    {
        return Encoding.ASCII.GetString(ReadLELength32PrefixedBytes());
    }

    public Vector2 ReadLEVector2()
    {
        var x = ReadLESingle();
        var y = ReadLESingle();

        return new Vector2(x, y);
    }

    public Vector3 ReadLEVector3()
    {
        var x = ReadLESingle();
        var y = ReadLESingle();
        var z = ReadLESingle();

        return new Vector3(x, y, z);
    }

    public Vector4 ReadLEVector4()
    {
        var x = ReadLESingle();
        var y = ReadLESingle();
        var z = ReadLESingle();
        var w = ReadLESingle();

        return new Vector4(x, y, z, w);
    }

    /// <summary>
    /// Reads a column-major 3x3 matrix but returns a functionally equivalent 4x4 matrix.
    /// </summary>
    public Matrix4x4 ReadLEColumnMajorMatrix3x3()
    {
        var matrix = new Matrix4x4();

        for (int columnIndex = 0; columnIndex < 4; columnIndex++)
        {
            for (int rowIndex = 0; rowIndex < 4; rowIndex++)
            {
                // If we're in the 3x3 part of the matrix, read values. Otherwise, use the identity matrix.
                if ((rowIndex <= 2) && (columnIndex <= 2))
                {
                    matrix[rowIndex, columnIndex] = ReadLESingle();
                }
                else
                {
                    matrix[rowIndex, columnIndex] = (rowIndex == columnIndex) ? 1 : 0;
                }
            }
        }

        return matrix;
    }

    /// <summary>
    /// Reads a row-major 3x3 matrix but returns a functionally equivalent 4x4 matrix.
    /// </summary>
    public Matrix4x4 ReadLERowMajorMatrix3x3()
    {
        var matrix = new Matrix4x4();

        for (int rowIndex = 0; rowIndex < 4; rowIndex++)
        {
            for (int columnIndex = 0; columnIndex < 4; columnIndex++)
            {
                // If we're in the 3x3 part of the matrix, read values. Otherwise, use the identity matrix.
                if ((rowIndex <= 2) && (columnIndex <= 2))
                {
                    matrix[rowIndex, columnIndex] = ReadLESingle();
                }
                else
                {
                    matrix[rowIndex, columnIndex] = (rowIndex == columnIndex) ? 1 : 0;
                }
            }
        }

        return matrix;
    }

    public Matrix4x4 ReadLEColumnMajorMatrix4x4()
    {
        var matrix = new Matrix4x4();

        for (int columnIndex = 0; columnIndex < 4; columnIndex++)
        {
            for (int rowIndex = 0; rowIndex < 4; rowIndex++)
            {
                matrix[rowIndex, columnIndex] = ReadLESingle();
            }
        }

        return matrix;
    }

    public Matrix4x4 ReadLERowMajorMatrix4x4()
    {
        var matrix = new Matrix4x4();

        for (int rowIndex = 0; rowIndex < 4; rowIndex++)
        {
            for (int columnIndex = 0; columnIndex < 4; columnIndex++)
            {
                matrix[rowIndex, columnIndex] = ReadLESingle();
            }
        }

        return matrix;
    }

    public Quaternion ReadLEQuaternionWFirst()
    {
        float w = ReadLESingle();
        float x = ReadLESingle();
        float y = ReadLESingle();
        float z = ReadLESingle();

        return new Quaternion(x, y, z, w);
    }

    public Quaternion ReadLEQuaternionWLast()
    {
        float x = ReadLESingle();
        float y = ReadLESingle();
        float z = ReadLESingle();
        float w = ReadLESingle();

        return new Quaternion(x, y, z, w);
    }

    #endregion

    #region Big Endian

    public bool ReadBEBool32()
    {
        return ReadBEUInt32() != 0;
    }

    public ushort ReadBEUInt16()
    {
        _reader.Read(_readBuffer, 0, 2);

        return (ushort)((_readBuffer[0] << 8) | _readBuffer[1]);
    }

    public uint ReadBEUInt32()
    {
        _reader.Read(_readBuffer, 0, 4);

        return ((uint)_readBuffer[0] << 24) | ((uint)_readBuffer[1] << 16) | ((uint)_readBuffer[2] << 8) | _readBuffer[3];
    }

    public ulong ReadBEUInt64()
    {
        _reader.Read(_readBuffer, 0, 8);

        return ((ulong)_readBuffer[0] << 56) | ((ulong)_readBuffer[1] << 48) | ((ulong)_readBuffer[2] << 40) | ((ulong)_readBuffer[3] << 32) | ((ulong)_readBuffer[4] << 24) | ((ulong)_readBuffer[5] << 16) | ((ulong)_readBuffer[6] << 8) | _readBuffer[7];
    }

    public short ReadBEInt16()
    {
        var buffer = new byte[2];
        _reader.Read(buffer, 0, 2);
        Array.Reverse(buffer);

        return BitConverter.ToInt16(_readBuffer, 0);
    }

    public int ReadBEInt32()
    {
        var buffer = new byte[4];
        _reader.Read(buffer, 0, 4);
        Array.Reverse(buffer);

        return BitConverter.ToInt32(_readBuffer, 0);
    }

    public long ReadBEInt64()
    {
        var buffer = new byte[8];
        _reader.Read(buffer, 0, 8);
        Array.Reverse(buffer);

        return BitConverter.ToInt32(_readBuffer, 0);
    }

    public float ReadBESingle()
    {
        var buffer = new byte[4];
        _reader.Read(buffer, 0, 4);
        Array.Reverse(buffer);

        return BitConverter.ToSingle(buffer, 0);
    }

    public double ReadBEDouble()
    {
        var buffer = new byte[8];
        _reader.Read(buffer, 0, 8);
        Array.Reverse(buffer);

        return BitConverter.ToDouble(buffer, 0);
    }

    public byte[] ReadBELength32PrefixedBytes()
    {
        var length = ReadBEUInt32();
        return _reader.ReadBytes((int)length);
    }

    public string ReadBELength32PrefixedASCIIString()
    {
        return Encoding.ASCII.GetString(ReadBELength32PrefixedBytes());
    }

    public Vector2 ReadBEVector2()
    {
        var x = ReadBESingle();
        var y = ReadBESingle();

        return new Vector2(x, y);
    }

    public Vector3 ReadBEVector3()
    {
        var x = ReadBESingle();
        var y = ReadBESingle();
        var z = ReadBESingle();

        return new Vector3(x, y, z);
    }

    public Vector4 ReadBEVector4()
    {
        var x = ReadBESingle();
        var y = ReadBESingle();
        var z = ReadBESingle();
        var w = ReadBESingle();

        return new Vector4(x, y, z, w);
    }

    /// <summary>
    /// Reads a column-major 3x3 matrix but returns a functionally equivalent 4x4 matrix.
    /// </summary>
    public Matrix4x4 ReadBEColumnMajorMatrix3x3()
    {
        var matrix = new Matrix4x4();

        for (int columnIndex = 0; columnIndex < 4; columnIndex++)
        {
            for (int rowIndex = 0; rowIndex < 4; rowIndex++)
            {
                // If we're in the 3x3 part of the matrix, read values. Otherwise, use the identity matrix.
                if ((rowIndex <= 2) && (columnIndex <= 2))
                {
                    matrix[rowIndex, columnIndex] = ReadBESingle();
                }
                else
                {
                    matrix[rowIndex, columnIndex] = (rowIndex == columnIndex) ? 1 : 0;
                }
            }
        }

        return matrix;
    }

    /// <summary>
    /// Reads a row-major 3x3 matrix but returns a functionally equivalent 4x4 matrix.
    /// </summary>
    public Matrix4x4 ReadBERowMajorMatrix3x3()
    {
        var matrix = new Matrix4x4();

        for (int rowIndex = 0; rowIndex < 4; rowIndex++)
        {
            for (int columnIndex = 0; columnIndex < 4; columnIndex++)
            {
                // If we're in the 3x3 part of the matrix, read values. Otherwise, use the identity matrix.
                if ((rowIndex <= 2) && (columnIndex <= 2))
                {
                    matrix[rowIndex, columnIndex] = ReadBESingle();
                }
                else
                {
                    matrix[rowIndex, columnIndex] = (rowIndex == columnIndex) ? 1 : 0;
                }
            }
        }

        return matrix;
    }

    public Matrix4x4 ReadBEColumnMajorMatrix4x4()
    {
        var matrix = new Matrix4x4();

        for (int columnIndex = 0; columnIndex < 4; columnIndex++)
        {
            for (int rowIndex = 0; rowIndex < 4; rowIndex++)
            {
                matrix[rowIndex, columnIndex] = ReadBESingle();
            }
        }

        return matrix;
    }

    public Matrix4x4 ReadBERowMajorMatrix4x4()
    {
        var matrix = new Matrix4x4();

        for (int rowIndex = 0; rowIndex < 4; rowIndex++)
        {
            for (int columnIndex = 0; columnIndex < 4; columnIndex++)
            {
                matrix[rowIndex, columnIndex] = ReadBESingle();
            }
        }

        return matrix;
    }

    public Quaternion ReadBEQuaternionWFirst()
    {
        float w = ReadBESingle();
        float x = ReadBESingle();
        float y = ReadBESingle();
        float z = ReadBESingle();

        return new Quaternion(x, y, z, w);
    }

    public Quaternion ReadBEQuaternionWLast()
    {
        float x = ReadBESingle();
        float y = ReadBESingle();
        float z = ReadBESingle();
        float w = ReadBESingle();

        return new Quaternion(x, y, z, w);
    }
    #endregion
}