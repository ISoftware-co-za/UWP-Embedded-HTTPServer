using System.Collections.Generic;
using Windows.Devices.Gpio;

namespace ISoftware.UWP.IOT
{
    public class GPIO
    {
        public GPIO()
        {
            _gpioController = GpioController.GetDefault();

            if (_gpioController != null)
            {
                _outputPins = new Dictionary<int, GpioPin>();

                _outputPins[5] = _gpioController.OpenPin(5);
                _outputPins[5].SetDriveMode(GpioPinDriveMode.Output);

                _outputPins[6] = _gpioController.OpenPin(6);
                _outputPins[6].SetDriveMode(GpioPinDriveMode.Output);

                _outputPins[13] = _gpioController.OpenPin(13);
                _outputPins[13].SetDriveMode(GpioPinDriveMode.Output);

                _outputPins[19] = _gpioController.OpenPin(19);
                _outputPins[19].SetDriveMode(GpioPinDriveMode.Output);
            }
        }

        public void On(int number)
        {
            if (_gpioController != null)
            {
                _outputPins[number].Write(GpioPinValue.Low);
            }
        }

        public void Off(int number)
        {
            if (_gpioController != null)
            {
                _outputPins[number].Write(GpioPinValue.High);
            }
        }

        private readonly GpioController _gpioController;
        private readonly Dictionary<int, GpioPin> _outputPins;
    }
}
