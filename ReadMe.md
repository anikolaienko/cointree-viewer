# CoinTree Viewer

WebApp for getting latest prices from CoinTree API.

## Features:
* Price is taken from CoinTree API, published to client and stored in database
* Every time the price is displayed, percentage of difference from the previous price is calculated.
* When the application starts, it displays the difference in price from the previous time it ran.
* Allows the user to input a price. If the live price is above the specified price, inform the user with green input highlighting when displaying the live price.

## TODO:
* Add browser notificaitons when users input price is higher/lower than five price. Currently there is no TypeScript notification library that I can easily use. Probably better to migrate to ES6, but that's a different story.

## To start the project from source code
```
dotnet restore
npm install
dotnet run
```

## To run Release build
In case there is something wrong with source code I included Release build in dist folder. In order to run Release build should be unpacked, and being in unpacked Release build folder you need to type:
```
dotnet CoinTreeViewer.dll
```
