# LoopBack Xamarin SDK example application 

This repository contains a mobile app that demonstrates the Loopback Xamarin SDK.

### What's in this repository? ###

* Server folder: Loopback server for the app.
* Client Folder: The ToDo app, created in Xamarin Studio with Xamarin Forms for iOS.

###  Run the server app

You can either run the LoopBack server app yourself, or connect to the demo app running at http://xamarindemo.strongloop.com.

**To run your own server app**:

1. Go to server folder: 
  ```$ cd server```
1. Install dependencies:
  ```npm install```
1.  Run the application:
  ```node .```

**To use StrongLoop's server app**

Alternatively, you can run the Xamarin client app against http://xamarindemo.strongloop.com.

Edit [LBXamarinSDK.cs](https://github.com/strongloop/loopback-example-xamarin/blob/master/Client/Todo%20App/TodoApp/TodoApp/LBXamarinSDK.cs) and change BASE_URL to `http://xamarindemo.strongloop.com/api`.

### Compile and run the client app

        Go to Client/ToDo App folder.
        Open TodoApp.sln project in Xamarin Studio.
        Change the API url setting line in the code to your local address. (Gateway.SetServerBaseURL())
        Compile and run.

### Links ###

* [Loopback](http://loopback.io)
* [SDK Repository](https://github.com/strongloop/loopback-sdk-xamarin)
* [Perfected Tech](http://perfectedtech.com)
* [Xamarin Studio](http://xamarin.com)
