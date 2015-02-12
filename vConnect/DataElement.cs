﻿using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using NCalc;

namespace vConnect
{
    /// <summary>
    /// This class describes a data element received from the car via BT.
    /// </summary>
    public class DataElement
    {
        private string obdPID = "";
        private string name = "";
        private int returnDataSize = 0;
        private byte[] returnData;
        private List<int> readBytes;
        private string valueToSend = "";
        private string equation = "";
        private List<byte> byteList;
        private int equVal1 = 0;
        private int equVal2 = 0;


        // This connection gets passed from the caller. It is the current connection.
        public BluetoothConnectionHandler BTConnection;

        // This defines the largest potential size of return data from the car. It should never
        //  really even get close to this value
        private int MAX_DATA_SIZE= 20;

        /// <summary>
        /// Constructor that defines name and PID and number of bytes explicitly 
        /// </summary>
        /// <param name="elementName">name of the element to be collected</param>
        /// <param name="PID">OBD PID to send to the car to get this value</param>
        /// <param name="numberBytesReturned">Number of bytes the car will return</param>
        /// <param name="btconnection">Current BT ConnectionHandler object.</param>
        public DataElement(string elementName, string PID, int numberBytesReturned, string eqn, BluetoothConnectionHandler btconnection)
        {
            name = elementName;
            obdPID = PID;
            returnData = new byte[numberBytesReturned];
            ReturnDataSize = numberBytesReturned;
            equation = eqn;
            BTConnection = btconnection;
        }

        /// <summary>
        /// Default constructor if no parameters passed
        /// </summary>
        public DataElement()
        {
            returnData = new byte[MAX_DATA_SIZE];
        }

        public bool RequestDataFromCar()
        {
            // /*
            if (BTConnection.Client.Connected)
            {
                Stream peerStream = BTConnection.Client.GetStream();
                string writeString = "01"+ ObdPID + "\r";

                byte[] writeCode = System.Text.Encoding.ASCII.GetBytes(writeString);
                peerStream.Write(writeCode, 0, writeCode.Length);

                System.Threading.Thread.Sleep(10000);

                
                byte[] rawTest =  new byte[20];
                
                
                peerStream.Read(rawTest, 0, rawTest.Length);
                string lengthOfByte = "This is the number of bytes: " + rawTest.Length.ToString();
                MessageBox.Show(lengthOfByte);
                MessageBox.Show("This is the actual data given: \n " + System.Text.Encoding.ASCII.GetString(rawTest));
               
                if (System.Text.Encoding.ASCII.GetString(rawTest).Contains("NO DATA"))
                {
                    // do whatever we are going to do if no data is received.


                }
                else
                {
                    List<byte> rawList = new List<byte>(rawTest);
                      byteList = new List<byte>();
                      string hexLiteral;
                      string hexLiteral2;
                
                    byte[] deezBytes = new byte[3];
                    //Skips repeated bytes.
                    if (ReturnDataSize == 1)
                      //  rawList.CopyTo(3, byteList, 0, 1 )
                        // bytes at 11 and 12 0 -> 00
                      {
                          hexLiteral = System.Text.Encoding.ASCII.GetString(rawTest, 11, 1) + System.Text.Encoding.ASCII.GetString(rawTest, 12, 1);
                      //  Buffer.BlockCopy(rawTest, 11, returnData , 0, 1);
                      MessageBox.Show("This is the actual data given: \n " + hexLiteral);
                      equVal1 = Convert.ToInt32(hexLiteral, 16);
                    

                    }
                    else if (ReturnDataSize == 2)
                        // bytes at 11,12,14,15
                        {
                            
                            hexLiteral = System.Text.Encoding.ASCII.GetString(rawTest, 11, 1) + System.Text.Encoding.ASCII.GetString(rawTest, 12, 1);
                            hexLiteral2 =  System.Text.Encoding.ASCII.GetString(rawTest, 14, 1) + System.Text.Encoding.ASCII.GetString(rawTest, 15, 1);
                        //Buffer.BlockCopy(rawTest, 11, returnData, 0, 2);
                        MessageBox.Show("This is the actual data given: \n " + hexLiteral);
                        MessageBox.Show("This is the actual data given: \n " + hexLiteral2);
                        equVal1 = Convert.ToInt32(hexLiteral, 16);
                        equVal2 = Convert.ToInt32(hexLiteral2, 16);


                    
                    }


                    byteList = new List<byte>();

                    // For testing.
                    // string codeString = System.Text.Encoding.ASCII.GetString(returnData);
                    //ValueToSend = System.Text.Encoding.ASCII.GetString(returnData);
                }
                
               
            }
            else 
            {
                MessageBox.Show("Lost BT Connection", "My Application",
                 MessageBoxButtons.OKCancel, MessageBoxIcon.Asterisk);
            }
            // */

            // byteList = new List<byte>(returnData);
            // byteList.Add(2);
            // ValueToSend = "1";
            return true;
            
        }


        public void FormatData()
        {
            // Create an expression with the equation specified.
            Expression expr = new Expression(Equation);

            // If the equation only has an "A"
            if (ReturnDataSize==1)
            {
                //expr.Parameters["A"] = byteList[1].ToString();
                expr.Parameters["A"] = equVal1;
            }

            // If the equation has A and B...
            else if (ReturnDataSize==2)
            {
                //expr.Parameters["A"] = byteList[1].ToString();
                //expr.Parameters["B"] = byteList[2];
                expr.Parameters["A"] = equVal1;
                expr.Parameters["B"] = equVal2;
            }
            
            // Should we go further?
            // else if...

            // evaluate the expression with the variables
            object answerToExpression = expr.Evaluate();

            // Store the formatted answer in the valueToSend variable.
            ValueToSend = answerToExpression.ToString();

            return;
        }

        public bool SendEmergencyCode()
        {
            return true;
        }

        // The following are C#'s way of using accessors (get and set). Call them with
        // Set:
        //      object.ObdPID = "02"
        // Get:
        //      object.ObdPID

        public string ObdPID { get { return obdPID; } set { obdPID = value; } }

        public string Name { get { return name; } set { name = value; } }

        public int ReturnDataSize { get { return returnDataSize; } set { returnDataSize = value; } }

        public byte[] ReturnData { get { return returnData; } set { returnData = value; } }

        public string ValueToSend { get { return valueToSend; } set { valueToSend = value; } }

        public string Equation { get { return equation; } set { equation = value; } }

    }

}








/*
           
               
			
			
               // Since the data received back repeats the mode (e.g. Mode 01) and the PID (e.g. "0D"), we will have a 
               // 	temporary holding space for the response.
               // 	For example, the response will be like "410D17".
               int recievedRaw = 0;
               recievedRaw = listenForResponse();

               // Depending on the size of the actual return data we are looking for, shift the raw response the given
               // 	number of bytes and store it in returnData. We do not want the first two bytes (e.g. 410D)
               // 	See Maciek's answer on http://stackoverflow.com/questions/1318933/c-sharp-int-to-byte for more details.
               returnData = (byte)(recievedRaw >> 8*getReturnDataSize());	
		
           }
		   
           */