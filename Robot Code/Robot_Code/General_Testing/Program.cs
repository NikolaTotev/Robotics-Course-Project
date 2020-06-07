﻿using System;
using System.Device.Gpio;
using System.Device.Pwm;
using System.Diagnostics.Tracing;
using System.IO.Ports;
using System.Threading;
using Iot.Device.ServoMotor;

namespace General_Testing
{
    class Program
    {
        //static SerialPort serialPort;
        //static int counter = 0;

        static void Main(string[] args)
        {

            if (args.Length > 0)
            {
                switch (args[0])
                {
                    case "Buttons":
                        TestButtons();
                        break;
                    case "Rotary":
                        TestEncoders();
                        break;

                    case "StpPT":
                        TestStepper(true);
                        break;

                    case "Stp":
                        TestStepper(false);
                        break;

                    case "Serv":
                        TestServos();
                        break;
                }
            }

            //float stepCounter = 0;
            //GpioController controller = new GpioController();
            //Console.WriteLine("STARTING GENERAL TESTING");

            //controller.OpenPin(18, PinMode.Output);

            //while (true)
            //{
            //    controller.Write(18, PinValue.High);
            //    Thread.Sleep(TimeSpan.FromSeconds(1));
            //    controller.Write(18, PinValue.Low);
            //    Thread.Sleep(TimeSpan.FromSeconds(1));
            //}


            //Thread.Sleep(TimeSpan.FromSeconds(20));

            //for (int i = 0; i < 400; i++)
            //{
            //    Console.WriteLine($"Write {i}");
            //    controller.Write(18, PinValue.High);
            //    Thread.Sleep(TimeSpan.FromSeconds(0.01));
            //    controller.Write(18, PinValue.Low);
            //    Thread.Sleep(TimeSpan.FromSeconds(0.01));
            //}

            //controller.Write(24, PinValue.Low);
            //for (int i = 0; i < 400; i++)
            //{
            //    Console.WriteLine($"Write {i}");
            //    controller.Write(25, PinValue.High);
            //    Thread.Sleep(TimeSpan.FromSeconds(0.01));
            //    controller.Write(25, PinValue.Low);
            //    Thread.Sleep(TimeSpan.FromSeconds(0.01));
            //}

        }

        static void TestButtons()
        {
            GpioController controller = new GpioController();
            Console.WriteLine("Starting button test");
            int[] buttonNumbers = new[] { 14, 15, 12, 16, 20, 21, 11, 5, 6, 26 };

            controller.OpenPin(7, PinMode.Output);
            controller.OpenPin(8, PinMode.Output);

            for (int i = 0; i < 4; i++)
            {
                controller.Write(7, PinValue.High);
                controller.Write(8, PinValue.High);
                Thread.Sleep(TimeSpan.FromSeconds(2));
                controller.Write(7, PinValue.Low);
                controller.Write(8, PinValue.Low);
            }

            foreach (var i in buttonNumbers)
            {
                controller.OpenPin(i, PinMode.InputPullUp);
                int counter = 0;
                while (true)
                {
                    Console.WriteLine($"Prepare to press button {i}");
                    Thread.Sleep(TimeSpan.FromSeconds(10));
                    Console.WriteLine($"Test for button {i} will begin in 5 seconds");
                    Thread.Sleep(TimeSpan.FromSeconds(10));

                    if (controller.Read(i) == PinValue.Low)
                    {
                        Console.WriteLine($"Button {i} has been pressed.");
                        break;
                    }
                    else
                    {
                        Console.WriteLine($"Button {i} was not detected to be pressed.\n You have one more attempt");
                        counter++;
                    }

                    if (counter >= 1)
                    {
                        break;
                    }

                }

            }

            Console.WriteLine("Button test complete");
        }

        static void TestEncoders()
        {
            Console.WriteLine("Starting encoder test");

            GpioController controller = new GpioController();
            int[] buttonNumbers = new[] { 4, 17, 27, 22, 10, 9 };
            foreach (var buttonNumber in buttonNumbers)
            {
                controller.OpenPin(buttonNumber, PinMode.InputPullUp);
            }

            controller.OpenPin(7, PinMode.Output);
            controller.OpenPin(8, PinMode.Output);

            for (int i = 0; i < 4; i++)
            {
                controller.Write(7, PinValue.High);
                controller.Write(8, PinValue.High);
                Thread.Sleep(TimeSpan.FromSeconds(2));
                controller.Write(7, PinValue.Low);
                controller.Write(8, PinValue.Low);
            }

            float counterA = 0;
            float counterB = 0;
            float counterC = 0;

            PinValue stateA;
            PinValue lastStateA;

            PinValue stateB;
            PinValue lastStateB;

            PinValue stateC;
            PinValue lastStateC;

            lastStateA = controller.Read(4);
            lastStateB = controller.Read(27);
            lastStateC = controller.Read(10);



            while (true)
            {

                Thread.Sleep(TimeSpan.FromSeconds(0.001));

                stateA = controller.Read(17);
                stateB = controller.Read(22);
                stateC = controller.Read(9);

                if (stateA != lastStateA)
                {
                    if (controller.Read(4) != stateA)
                    {
                        counterA += 2.5f;
                    }
                    else
                    {
                        counterA -= 2.5f;
                    }

                    if (Math.Abs(counterA % 1) < 0.01)
                    {
                        Console.WriteLine($"Counter A: {counterA}");
                    }
                }

                if (stateB != lastStateB)
                {
                    if (controller.Read(27) != stateB)
                    {
                        counterB += 2.5f;
                    }
                    else
                    {
                        counterB -= 2.5f;
                    }

                    if (Math.Abs(counterB % 1) < 0.01)
                    {
                        Console.WriteLine($"Counter B: {counterB}");
                    }
                }

                if (stateC != lastStateC)
                {
                    if (controller.Read(10) != stateC)
                    {
                        counterC += 2.5f;
                    }
                    else
                    {
                        counterC -= 2.5f;
                    }

                    if (Math.Abs(counterC % 1) < 0.01)
                    {
                        Console.WriteLine($"Counter C: {counterC}");
                    }
                }

                lastStateA = stateA;
                lastStateB = stateB;
                lastStateC = stateC;
            }
        }
        static void TestStepper(bool pinTest)
        {
            Console.WriteLine("Starting stepper motor test.");

            GpioController controller = new GpioController();
            int jointAStep = 4;
            int jointADir = 17;
            int jointBStep = 27;
            int jointBDir = 22;

            
                controller.OpenPin(jointAStep, PinMode.Output);
                controller.OpenPin(jointADir, PinMode.Output);
                controller.OpenPin(jointBStep, PinMode.Output);
                controller.OpenPin(jointBDir, PinMode.Output);
            

            Console.WriteLine("ENTERING PIN TEST MODE");
            Console.WriteLine("Testing motor A");

            controller.Write(jointADir, PinValue.Low);
            for (int i = 0; i < 400; i++)
            {
                Console.WriteLine($"Write to A {i}");
                controller.Write(jointAStep, PinValue.High);
                Thread.Sleep(TimeSpan.FromSeconds(0.01));
                controller.Write(jointAStep, PinValue.Low);
                Thread.Sleep(TimeSpan.FromSeconds(0.01));
            }

            controller.Write(jointADir, PinValue.High);
            for (int i = 0; i < 400; i++)
            {
                Console.WriteLine($"Write to A {i}");
                controller.Write(jointAStep, PinValue.High);
                Thread.Sleep(TimeSpan.FromSeconds(0.01));
                controller.Write(jointAStep, PinValue.Low);
                Thread.Sleep(TimeSpan.FromSeconds(0.01));
            }

            Thread.Sleep(TimeSpan.FromSeconds(2));


            Console.WriteLine("Testing motor B");

            controller.Write(jointBDir, PinValue.Low);
            for (int i = 0; i < 200; i++)
            {
                Console.WriteLine($"Write to B {i}");
                controller.Write(jointBStep, PinValue.High);
                Thread.Sleep(TimeSpan.FromSeconds(0.01));
                controller.Write(jointBStep, PinValue.Low);
                Thread.Sleep(TimeSpan.FromSeconds(0.01));
            }

            controller.Write(jointBDir, PinValue.High);
            for (int i = 0; i < 200; i++)
            {
                Console.WriteLine($"Write to B {i}");
                controller.Write(jointBStep, PinValue.High);
                Thread.Sleep(TimeSpan.FromSeconds(0.01));
                controller.Write(jointBStep, PinValue.Low);
                Thread.Sleep(TimeSpan.FromSeconds(0.01));
            }
        }

        static void TestServos()
        {
        }
    }
}




//float counter = 0;
//PinValue state;
//PinValue lastState;

//lastState = controller.Read(10);
//while (true)
//{
//    state = controller.Read(10);

//    if (state != lastState)
//    {
//        if (controller.Read(11) != state)
//        {
//            counter+=0.5f;
//        }
//        else
//        {
//            counter-=0.5f;
//        }

//        Console.WriteLine($"Counter: {counter}, State: {state}, Last State: {lastState}");

//    }

//    Thread.Sleep(TimeSpan.FromSeconds(0.001));
//    lastState = state;

//    if (controller.Read(9) == PinValue.Low)
//    {
//        Console.WriteLine("I'VE BEEN CLICKED");
//    }


//}



//    PinValue state;
//    PinValue lastState;
//    lastState = controller.Read(11);

//    PinValue emergencyStop = PinValue.High;

//    Console.Write("Port no: ");
//    Console.Write("baudrate: ");

//    serialPort = new SerialPort("/dev/ttyACM0", 9600); //Set the read/write timeouts    
//    serialPort.ReadTimeout = 1500;
//    serialPort.WriteTimeout = 1500;
//    serialPort.Open();
//    serialPort.WriteLine("80");
//    while (emergencyStop != PinValue.Low)
//    {
//    emergencyStop = controller.Read(26);
//    if (controller.Read(10) == PinValue.Low)
//    {
//        stepCounter = 80;
//        serialPort.WriteLine("80");
//        Thread.Sleep(TimeSpan.FromMilliseconds(15));
//    }
//    Read();
//    Thread.Sleep(TimeSpan.FromSeconds(0.001));

//    state = controller.Read(11);

//    if (state != lastState)
//    {
//        if (controller.Read(9) != state)
//        {
//            stepCounter += 2.5f;
//        }
//        else
//        {
//            stepCounter -= 2.5f;
//        }

//        if (Math.Abs(stepCounter % 1) < 0.01)
//        {
//            Console.WriteLine($"Step: {stepCounter}");
//            serialPort.WriteLine($"{stepCounter}");
//            Thread.Sleep(TimeSpan.FromMilliseconds(15));
//        }

//        if (Math.Abs(stepCounter - 360) < 0.0001 || Math.Abs(stepCounter - (-360)) < 0.0001)
//        {
//            stepCounter = 0;
//        }
//    }

//    Thread.Sleep(TimeSpan.FromSeconds(0.001));
//    lastState = state;
//    }
//    serialPort.Close();
//}
//public static void Read()
//{
//try
//{
//    if (serialPort.BytesToRead > 0)
//    {
//        string message = serialPort.ReadLine();
//        Console.WriteLine(message);
//    }

//}
//catch (TimeoutException)
//{
//}
//}
