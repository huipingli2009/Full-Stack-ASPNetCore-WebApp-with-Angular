# PHO Web App

> Following <https://github.com/noffle/art-of-readme> outline
> <https://github.com/noffle/common-readme>
> <https://github.com/richardlitt/standard-readme>

TODO: Additional info to follow, such as a simple intro, environment information, and some links to documentation.


## org.cchmc.pho.webapp

Angular JS application

### Setting App Version
Please refer to this guide
https://docs.npmjs.com/cli/version

> NOTE: invoke this in the project directory of /org.cchmc.pho.webapp

```console
npm version patch
-- or --
npm version patch -git-tag-version false
```
This will update the version number in the package.json

### Killing 4200 port on a mac
```console
> npx kill-port 4200
```

## Local Development

###

###  JSON-Server

> this is installed locally as a dev package.
> https://egghead.io/lessons/npm-use-npx-to-run-locally-installed-node-modules

```console
//in the org.cchmc.pho.webapp folder run the following
//Will need to run npm i before this command can be invoked and/or build the web app first
> npx json-server --watch db.json --routes db-routes.json
```




## Starting a New Controller
### Adding the Class
When adding a new Controller, add it to the API project (`org.cchmc.pho.api`) in the `Controllers` folder. Be sure the new Controller is in the `org.cchmc.pho.api.Controllers`
namespace. Apply the `ApiController` and `Route` attributes (use the `AlertController` as an example). Methods in the controller
should be async when possible (thus having the signature `public async Task<IActionResult> MethodName(args)`).

### Constructor
Be sure the constructor is dependency-injection (DI) ready. Following the example in the `AlertController`, the
logger, AutoMapper reference, and reference to the Data Access Layer should be available. Assign the passed references
to private variables in your new Controller class. The generic for the logger should be the name of your new class.

### Routes
#### Reads
For retrieving an item or a list of items, use the `HttpGet` attribute and provide the proper path. Sections of the path wrapped in `{}`
are parameters in the method signature. For example, `[HttpGet("users/{userId}")]` assumes the method will accept a parameter named
`userId` of type `string`. Parameters in the route will always be strings.

#### Adds/Updates/Deletes
For modifying data, use the `HttpPost`, `HttpPut`, or `HttpDelete` attributes, providing the proper path and parameters. The
`[FromBody]` tag can be put before a parameter to specify that the object is being posted as part of the HTTP request,
rather than part of the URL path. For example, `public void AddUser([FromBody] User user)` indicates that the method will
expect the caller to include a valid `User` object as part of the request.

#### SwaggerResponses
This app is using Swagger for easier API consumption. The `SwaggerResponse` attributes indicate the possible HTTP status codes
and the resulting return type. For example, `[SwaggerResponse(200, type: typeof(User))]` returns a `User` object when the
method is successful, while `[SwaggerResponse(500, type: typeof(string))]` returns a string (possibly an error message) when
a 500 status code is returned.

### View Models
The `org.cchmc.pho.api` project has a `ViewModels` folder for the objects returned and accepted by the Controllers. Do not
have a Controller method that takes or returns a Data Model object.

#### AutoMapper
This solution uses `AutoMapper` (<https://automapper.org/>) for translating from Data Models to View Models. When creating a new
Data Model/View Model pair, you need to tell AutoMapper how to map from one to the next. To do so, create a new class in the
`Mappings` folder in the API project (by convention, I add one mapping class per controller). Be sure the new class is in the
`org.cchmc.pho.api.Mappings` namespace. You can follow the example in `AlertMappings.cs`. In the constructor in your mappings
class, you need to call `CreateMap<T1, T2>()` where `T1` is your Data Model (the source) and `T2` is your View Model (the
destination). If you need to translated from the View Model to the Data Model, you would call `CreateMap<T2, T1>()`.


## Starting a new Data Access Layer class
The core project (`org.cchmc.pho.core`) contains the Data Access Layer (DAL) classes for data retrieval and modifications. The
`Interfaces` folder contains the interfaces for all the classes; if making a new DAL class, be sure it has a corresponding
interface so that it can be dependency-injected. Add your implementation to the `DataAccessLayer` folder.

### Data Models
The core project has a `DataModels` folder for putting all of your objects that the Data Layer receives or returns.



---



## Intro (what is it)

- Simple introduction
- What it is
- What it does


## Example

- Examples of it running
- Screenshots
- Snippets


## How to use it

- Installation
- Setup
- Samples
- APIs


## Any other relevant data

- Advance features
- Advance commands
- Reference material

