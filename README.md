# OTP CLI

A simple wrapper around the [Otp.NET](https://github.com/kspearrin/Otp.NET) library to generate OTP values from the command line.

----

## Configuration

1. Create a file in your home directory named `.otpcli`
   - Windows: `%USERPROFILE%\.otpcli`
   - Linux/Mac: `$HOME/.otpcli`
1. Within this file enter your secret key encoded as a Base64 string
1. (Optional) Add the location of the executable to your system path

----

## Usage

Run `otpcli` from whatever shell/terminal you use.

The only output is your current OTP value.

There are no options, flags, parameters, or any other way to configure this utility.  Honestly, it took longer to write this README than it did to build this utility.
