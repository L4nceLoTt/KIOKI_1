﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace KIOKI_1.Classes
{
    public static class Algs
    {
        /* общие методы */

        private static char[,] GetEmptyMatrix(int rows, int columns)
        {
            char[,] tmp = new char[rows, columns];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    tmp[i, j] = '\0';
                }
            }

            return tmp;
        }
        private static void PrintMatrix(char[,] matrix)
        {
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    Console.Write(matrix[i, j] + " ");
                }
                Console.Write('\n');
            }
        }

        /* методы для изгороди */

        private static char[,] getHedge(int messageLength, int key)
        {
            char[,] hedge = GetEmptyMatrix(key, messageLength);

            int row = 0;
            int delta = 0;

            for (int i = 0; i < messageLength; i++)
            {
                if (row == 0) delta = 1;
                else if (row == key - 1) delta = -1;

                hedge[row, i] = '*';

                row += delta;
            }

            return hedge;
        }
        private static char[,] fillHedge(char[,] hedge, string message)
        {
            int elementAt = 0;
            for (int i = 0; i < hedge.GetLength(0); i++)
            {
                for (int j = 0; j < hedge.GetLength(1); j++)
                {
                    if (hedge[i, j] == '*')
                    {
                        hedge[i, j] = message[elementAt];
                        elementAt++;
                    }
                }
            }

            return hedge;
        }
        private static string getDecryptedMessage(char[,] hedge, int key)
        {
            string decryptedMessage = null;

            int row = 0;
            int delta = 0;

            for (int i = 0; i < hedge.GetLength(1); i++)
            {
                if (row == 0) delta = 1;
                else if (row == key - 1) delta = -1;

                if (hedge[row, i] != '\0') decryptedMessage += hedge[row, i];

                row += delta;
            }

            return decryptedMessage;
        }

        public static string hedgeEncrypt(string message, int key)
        {
            if (key < 2 || message.Length < 2) return null;

            string encryptedMessage = null;

            char[,] matrix = GetEmptyMatrix(key, message.Length);

            int row = 0;
            int delta = 0;

            for (int i = 0; i < message.Length; i++)
            {
                if (row == 0) delta = 1;
                else if (row == key - 1) delta = -1;

                matrix[row, i] = message[i];

                row += delta;
            }

            for (int i = 0; i < key; i++)
            {
                for (int j = 0; j < message.Length; j++)
                {
                    if (matrix[i, j] != '\0') encryptedMessage += matrix[i, j];
                }
            }

            return encryptedMessage;
        }
        public static string hedgeDecrypt(string message, int key)
        {
            if (key < 2 || message.Length < 2) return null;

            char[,] matrix = getHedge(message.Length, key);

            matrix = fillHedge(matrix, message);

            return getDecryptedMessage(matrix, key);
        }

        /* методы для ключевой фразы */

        private static char[,] GetFilledKeyPhraseMatrix(char[,] matrix, string message)
        {
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    if ((i * matrix.GetLength(1)) + j < message.Length) matrix[i, j] = message[(i * matrix.GetLength(1)) + j];
                    else matrix[i, j] = ' ';
                }
            }

            return matrix;
        }
        private static int[] GetRefKeyPhraseMatrix(string key)
        {
            int[] queue = new int[key.Length];

            char[] keyPhrase = key.ToCharArray();

            int counter = 9999;
            while (queue.Contains(0))
            {
                int index = key.IndexOf(key.Max());

                queue[index] = counter;

                counter--;

                keyPhrase[index] = '\0';

                key = new string(keyPhrase);
            }

            int min = queue.Min();

            for (int i = 0; i < queue.GetLength(0); i++)
            {
                queue[i] -= min;
            }

            return queue;
        }
        private static string GetEncryptedKeyPraseMessage(int[] refMatrix, char[,] msg)
        {
            string result = "";

            char[,] encryptedMessage = new char[msg.GetLength(0), msg.GetLength(1)];

            for (int i = 0; i < msg.GetLength(1); i++)
            {
                for (int j = 0; j < msg.GetLength(0); j++)
                {
                    encryptedMessage[j, refMatrix[i]] = msg[j, i];
                }
            }

            for (int i = 0; i < msg.GetLength(0); i++)
            {
                for (int j = 0; j < msg.GetLength(1); j++)
                {
                    result += encryptedMessage[i, j];
                }
            }

            return result;
        }
        private static string GetDecryptedKeyPhraseMessage(int[] refMatrix, char[,] msg)
        {
            string result = "";

            char[,] decryptedMessage = new char[msg.GetLength(0), msg.GetLength(1)];

            for (int i = 0; i < msg.GetLength(1); i++)
            {
                for (int j = 0; j < msg.GetLength(0); j++)
                {
                    decryptedMessage[j, i] = msg[j, refMatrix[i]];
                }
            }

            for (int i = 0; i < msg.GetLength(0); i++)
            {
                for (int j = 0; j < msg.GetLength(1); j++)
                {
                    result += decryptedMessage[i, j];
                }
            }

            return result;
        }

        public static string KeyPhraseEncrypt(string message, string key)
        {
            if (message.Length < 1 || message == null) return "";

            int[] refMatrix = GetRefKeyPhraseMatrix(key);
            char[,] msg = GetEmptyMatrix((message.Length % key.Length) == 0 ? message.Length / key.Length : (message.Length / key.Length) + 1, key.Length);

            msg = GetFilledKeyPhraseMatrix(msg, message);


            return GetEncryptedKeyPraseMessage(refMatrix, msg);
        }
        public static string KeyPhraseDecrypt(string message, string key)
        {
            if (message.Length < 1) return "";

            int[] refMatrix = GetRefKeyPhraseMatrix(key);
            char[,] msg = GetEmptyMatrix(message.Length / key.Length, key.Length);

            msg = GetFilledKeyPhraseMatrix(msg, message);

            return GetDecryptedKeyPhraseMessage(refMatrix, msg);
        }

        /* методы для поворачивающейся решётки */

        private static char[,] RotateGrate(char[,] grate)
        {
            char[,] rotated = new char[grate.GetLength(1), grate.GetLength(0)];

            for (int i = 0; i < grate.GetLength(0); i++)
            {
                for (int j = 0; j < grate.GetLength(1); j++)
                {
                    rotated[j, grate.GetLength(1) - i - 1] = grate[i, j];
                }
            }

            return rotated;
        }
        private static char[,] GetFilledGrate(char[,] grate, string message)
        {
            for (int i = 0; i < grate.GetLength(0); i++)
            {
                for (int j = 0; j < grate.GetLength(1); j++)
                {
                    grate[i, j] = message[(i * grate.GetLength(0)) + j];
                }
            }

            return grate;
        }
        private static string EncryptSubstring(string substring, CustomButton[] cells)
        {
            string encryptedSubstring = null;

            char[,] grate = GetEmptyMatrix((int)Math.Sqrt(cells.Length * 4), (int)Math.Sqrt(cells.Length * 4));

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < cells.Length; j++)
                {
                    if ((i * cells.Length) + j > substring.Length - 1) grate[cells[j].row, cells[j].column] = ' ';
                    else grate[cells[j].row, cells[j].column] = substring[(i * cells.Length) + j];
                }
                grate = RotateGrate(grate);
            }

            for (int i = 0; i < (int)Math.Sqrt(cells.Length * 4); i++)
            {
                for (int j = 0; j < (int)Math.Sqrt(cells.Length * 4); j++)
                {
                    encryptedSubstring += grate[i, j];
                }
            }

            return encryptedSubstring;
        }
        private static string DecryptSubstring(string substring, CustomButton[] cells)
        {
            string decryptedSubstring = null;

            char[,] grate = GetEmptyMatrix((int)Math.Sqrt(cells.Length * 4), (int)Math.Sqrt(cells.Length * 4));

            for (int i = 0; i < (int)Math.Sqrt(cells.Length * 4); i++)
            {
                for (int j = 0; j < (int)Math.Sqrt(cells.Length * 4); j++)
                {
                    grate[i, j] = substring[(i * (int)Math.Sqrt(cells.Length * 4)) + j];
                }
            }

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < cells.Length; j++)
                {
                    decryptedSubstring += grate[cells[j].row, cells[j].column];
                }
                grate = RotateGrate(grate);
            }

            return decryptedSubstring;
        }

        public static string RotatingGrateEncrypt(string message, CustomButton[] cells)
        {
            string encryptedString = null;
            int iterations = message.Length % ((cells.Length) * 4) > 0 ? message.Length / ((cells.Length) * 4) + 1 : message.Length / ((cells.Length) * 4);

            for (int i = 0; i < iterations; i++)
            {
                if (message.Length - i * (cells.Length * 4) < cells.Length * 4) encryptedString += EncryptSubstring(message.Substring(i * (cells.Length * 4)), cells);
                else encryptedString += EncryptSubstring(message.Substring(i * (cells.Length * 4), cells.Length * 4), cells);
            }

            return encryptedString;
        }
        public static string RotatingGrateDecrypt(string message, CustomButton[] cells)
        {
            string decryptedString = null;
            int iterations = message.Length % ((cells.Length) * 4) > 0 ? message.Length / ((cells.Length) * 4) + 1 : message.Length / ((cells.Length) * 4);

            for (int i = 0; i < iterations; i++)
            {
                decryptedString += DecryptSubstring(message.Substring(i * (cells.Length * 4)), cells);
            }

            return decryptedString;
        }

        /* методы для цезаря */

        public static string CaesarEncrypt(string message, int key)
        {
            string encryptedMessage = null;

            for (int i = 0; i < message.Length; i++) 
            {
                if (char.IsLetter(message[i]))
                {
                    encryptedMessage += (char)((int)message[i] > 1071 ? 1072 + (((int)message[i] - 1072 + key) % 32) : 1040 + (((int)message[i] - 1040 + key) % 32));
                }
                else encryptedMessage += message[i];
            }

            return encryptedMessage;
        }
        public static string CaesarDecrypt(string message, int key)
        {
            string decryptedMessage = null;

            for (int i = 0; i < message.Length; i++)
            {
                if (char.IsLetter(message[i]))
                {
                    decryptedMessage += (char)((int)message[i] > 1071 ? 1072 + (((int)message[i] - 1072 + 32 - key) % 32) : 1040 + (((int)message[i] - 1040 + 32 - key) % 32));
                }
                else decryptedMessage += message[i];
            }

            return decryptedMessage;
        }

        /* методы для децимации */

        public static int GreatestCommonDivisor(int a, int b)
        {
            while (a != 0 && b != 0)
            {
                if (a > b) a = a % b;
                else b = b % a;
            }

            return a + b;
        }

        public static string DecimationEncrypt(string message, int key)
        {
            string encryptedMessage = null;

            for (int i = 0; i < message.Length; i++)
            {
                if (char.IsLetter(message[i]))
                {
                    encryptedMessage += (char)((int)message[i] > 1071 ? 1072 + ((((int)message[i] - 1072) * key) % 32) : 1040 + ((((int)message[i] - 1040) * key) % 32));
                }
                else encryptedMessage += message[i];
            }

            return encryptedMessage;
        }
    }
}