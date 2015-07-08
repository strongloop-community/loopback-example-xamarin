# LoopBack Xamarin SDK example application 

This repository contains a mobile app that demonstrates the Loopback Xamarin SDK.

### What's in this repository? ###

* Server folder: Loopback server for the app.
* Client Folder: The ToDo app, created in Xamarin Studio with Xamarin Forms for iOS.

### How do I get set up? ###

* Run the server

        Go to server folder.
        Run `npm install` in shell.
        Run `node .` in shell.

**NOTE**:  If you don't want to install Node.js and StrongLoop, you can run the Xamarin client app against http://xamarindemo.strongloop.com.

* Compile and run the App

        Go to Client/ToDo App folder.
        Open TodoApp.sln project in Xamarin Studio.
        Change the API url setting line in the code to your local address. (Gateway.SetServerBaseURL())
        Compile and run.

### Links ###

* [Loopback](http://loopback.io)
* [SDK Repository](https://github.com/strongloop/loopback-sdk-xamarin)
* [Perfected Tech](http://perfectedtech.com)
* [Xamarin Studio](http://xamarin.com)
