# Hypermedia Client Lib
_Experimental_ project to create a Client which allows typed access to a Rest Api.
- Fluent Link navigation
- Transparent Link and Action resolving with deserialization
- Action handling

There is still a lot to do:
- Pass HttpClient to resolver so lib user has access
- Exception handling
- Error responses by the api (http, authorization, hypermediaparsing and api), problem json, status codes
- (more) Authentification/Authorization
- Use chache info from header so client may cache resolved documents
- Allow actions with schemas OR fields
- Add a attribute wich holds the json property name which should be desrialized into a hco property
...

Assumptions:
- Siren (single paramert json objects like in the CarShack demo)
- Deterministic Links, a link will always lead to the same class of ressource. E.g. a Customer.