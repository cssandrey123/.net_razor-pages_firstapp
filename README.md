A base project that use a cocktail recipes API where you can filter drinks by category or you ca specifically search for a drink by inserting its name into the input search field.

This project implements serverside caching to cache the responses. In order to see that, open the applicaiton output console inside visual studio. There will be outputted  if
the specific url of the api that you are hitting makes an http call or it's coming from cache. You can see that at the first load, the drinks response is fetched from
the http and is saved in cache. At next reload, the response will be sent from cache.
