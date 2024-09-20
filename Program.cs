using CoffeeShop;
using CoffeeShop.Prowiders;


var httpListener = new CoffeeShopHttpListener();

await httpListener.ListenAsync();