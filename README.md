# ATM Banking App

This is a full-stack application that simulates an ATM system. It has the following functionality:
- User registration
- User login
- View balance and transactions
- Deposit money
- Withdraw money

The frontend is made using React Native + Expo, and the backend is made with ASP.NET with the help of the Entity Framework Core.

# Setup

The following instructions will explain how to setup the app (on Android) and the backend (using Docker.)

## Backend

Before starting, make sure you have [Docker](https://docs.docker.com/get-started/get-docker/) and [Docker Compose](https://docs.docker.com/compose/install) installed.

First, clone the repo:
```console
git clone https://github.com/ert112358/ATM_Project
```
Then, navigate to the directory:
```console
cd ATM_Project/ATM_BackEnd/ATM_BackEnd
```
And finally, run the container:
```console
docker compose up -d
```

## Frontend


Before starting, make sure you have [Node.JS](https://nodejs.org/en) installed on your PC and [Expo Go](https://expo.dev/go) installed on your mobile device.

First, clone the repo:
```console
git clone https://github.com/ert112358/ATM_Project
```
Then, navigate to the directory:
```console
cd ATM_Project/ATM_FrontEnd
```
And finally, run the development server:
```console
npx expo start
```
# API

After running the container, the API will be exposed on port 5001. Here's a table explaining the API calls and what they do:
- `hostname:5001/api/register?username=USER&password=PASSWORD`
This API call will add the supplied user to the database, if it doesn't already exist with the same name and only if the password is strong enough.
- `hostname:5001/api/login?username=USER&password=PASSWORD`
This API call will check if the supplied username and password are valid and if they are, it will respond with the access token (32 bytes long, randomly generated) associated with that user.
- `hostname:5001/api/viewbalance?token=TOKEN`
This API call return the username, balance and full list of transactions that the user made.
- `hostname:5001/api/withdraw?token=TOKEN&amount=NUM`
This API call will subtract the specified amount of money from the user's balance.
- `hostname:5001/api/deposit?token=TOKEN&amount=NUM`
This API call will add the specified amount of money to the user's balance.
- `hostname:5001/api/changepassword?username=USERNAME&oldpassword=OLDPW&newpassword=NEWPW`
This API call will change the user's password. Like the `register` function, it will check if the password is strong enough, and if the change is successful, it sends a new access token to the caller.
