# Inertia.js ASP.NET Adapter

[![NuGet](https://img.shields.io/nuget/v/InertiaNetCore?style=flat-square&color=blue)](https://www.nuget.org/packages/InertiaNetCore)
[![NuGet](https://img.shields.io/nuget/dt/InertiaNetCore?style=flat-square)](https://www.nuget.org/packages/InertiaNetCore)
[![License](https://img.shields.io/github/license/mergehez/InertiaNetCore?style=flat-square)](https://github.com/mergehez/InertiaNetCore/blob/main/LICENSE)

This library is a fork of [kapi2289/InertiaCore](https://github.com/kapi2289/InertiaCore). (Last commit: Aug 18, 2023)

Some errors were fixed, and unnecessary dependencies were removed. The library will be maintained and updated whenever necessary.

It is compatible with .NET 7 and .NET 8. As soon as .NET 9 is released, the library will be updated to support it.

Feel free to contribute to the project by creating issues or pull requests.

## Table of contents

- [Demo](#demo)
- [Installation](#installation)
- [Getting started](#getting-started)
- [Configuration](#configuration)
- [Usage](#usage)
  * [Frontend](#frontend)
  * [Backend](#backend)
- [Features](#features)
  * [Shared data](#shared-data)
  * [Flash Messages](#flash-messages)
  * [Server-side rendering](#server-side-rendering)
  * [Vite helper](#vite-helper)
    - [Examples](#examples-1)

## Demo

Demo is available at https://inertianetcore-d5c7hcggg7afdqg0.germanywestcentral-01.azurewebsites.net/

If you want to see how it exactly works, you can clone this repository and play with [InertiaNetCore.Demo](InertiaNetCore.Demo). It contains a simple Vue.js frontend and an ASP.NET Core backend.

## Installation

1. Using Package Manager: `PM> Install-Package InertiaNetCore`
2. Using .NET CLI: `dotnet add package InertiaNetCore`
3. Using NuGet Package Manager: search for `InertiaNetCore`

## Getting started

You need to add few lines to the `Program.cs` or `Starup.cs` file.

```csharp
using InertiaNetCore.Extensions;

[...]

builder.Services.AddInertia();
builder.Services.AddViteHelper(); // assuming you are using Vite

[...]

app.UseInertia();

```

## Configuration

Both `AddInertia` and `AddViteHelper` methods have optional parameters to configure the library.

For example, you can change JSON serializer settings to use `Newtonsoft.Json` instead of `System.Text.Json`.

```csharp
builder.Services.AddInertia(options =>
{
    options.JsonSerializerSettings = new JsonSerializerSettings
    {
        ContractResolver = new CamelCasePropertyNamesContractResolver(),
        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
    };
    
    options.JsonSerializeFn = model => JsonConvert.SerializeObject(model, options.JsonSerializerSettings);
});
```


Visit the [InertiaOptions](InertiaNetCore/Models/InertiaOptions.cs) and [ViteOptions](InertiaNetCore/Models/ViteOptions.cs) classes to see all available options.

## Usage

### Frontend

Create a file `/Views/App.cshtml`.

```html
@using InertiaNetCore
@using InertiaNetCore.Utils

<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="utf-8"/>
    <meta name="viewport" content="width=device-width, initial-scale=1.0"/>
    <title inertia>My App</title>
</head>

<body>
@await Inertia.Html(Model)

@Vite.Input("src/app.ts")
</body>
</html>
```
> [!NOTE]
> Default root view is `App.cshtml` but you can change it by setting `RootView` in `AddInertia` method in `Program.cs`.

### Backend

To pass data to a page component, use `Inertia.Render()`.

```csharp
[Route("about")]
public IActionResult About()
{
    return Inertia.Render("pages/PageAbout", new InertiaProps
    {
        ["Name"] = "InertiaNetCore",
        ["Version"] = Assembly.GetAssembly(typeof(Inertia))?.GetName().Version?.ToString()
    });
}
```

To make a form endpoint, remember to add `[FromBody]` to your model parameter, because the request data is passed using
JSON.

```csharp
[HttpPost]
public async Task<IActionResult> Create([FromBody] Post post)
{
    if (!ModelState.IsValid)
    {
        // The validation errors are passed automatically.
        return await Index();
    }
    
    _context.Add(post);
    await _context.SaveChangesAsync();
    
    return RedirectToAction("Index");
}
```

## Features

### Shared data

You can add some shared data to your views using for example middlewares:

```csharp
using InertiaNetCore;
using InertiaNetCore.Extensions;
using InertiaNetCore.Models;

[...]

app.Use(async (context, next) =>
{
    Inertia.Share( new InertiaProps
    {
        ["Auth"] = new InertiaProps
        {
            ["Token"] = "123456789",
            ["Username"] = "Mergehez",
        }
    });
            
    await next();
});

// you can also use AddInertiaSharedData extension method to do the same thing
app.AddInertiaSharedData(httpContext => new InertiaProps
{
    ["Auth"] = new InertiaProps
    {
        ["Token"] = "123456789",
        ["Username"] = "Mergehez",
    }
});
```

### Flash Messages

You can add flash messages to your responses using the `Inertia.Back(url).WithFlash(...)` method.
    
```csharp
[HttpDelete("{id:int}")]
public async Task<IActionResult> Destroy(int id)
{
    /// find user

    if (!user.IsAdmin)
        return Inertia.Back().WithFlash("error", "Admins cannot be deleted.");
    
    // delete user
    
    return Inertia.Back().WithFlash("success", "User deleted.");
}
```


### Server-side rendering

If you want to enable SSR in your Inertia app, remember to add `Inertia.Head()` to your layout:

```diff
@using InertiaNetCore
@using InertiaNetCore.Utils

<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="utf-8"/>
    <meta name="viewport" content="width=device-width, initial-scale=1.0"/>
    <title inertia>My App</title>

+   @await Inertia.Head(Model)
</head>

<body>
@await Inertia.Html(Model)

@Vite.Input("src/app.ts")
</body>
</html>
```

and enable the SSR option in `Program.cs`.

```csharp
builder.Services.AddInertia(options =>
{
    options.SsrEnabled = true;
    
    // You can optionally set a different URL than the default.
    options.SsrUrl = "http://127.0.0.1:13714/render"; // default
});
```

### Vite Helper

A Vite helper class is available to automatically load your generated styles or scripts by simply using the `@Vite.Input("src/main.tsx")` helper. You can also enable HMR when using React by using the `@Vite.ReactRefresh()` helper. This pairs well with the `laravel-vite-plugin` npm package.

To get started with the Vite Helper, you have to use the `AddViteHelper` extension method in `Program.cs`.

```csharp
using InertiaNetCore.Extensions;

[...]

builder.Services.AddViteHelper();

// Or with options (default values shown)

builder.Services.AddViteHelper(options =>
{
    options.PublicDirectory = "wwwroot";
    options.BuildDirectory = "build";
    options.HotFile = "hot";
    options.ManifestFilename = "manifest.json";
});
```

#### Examples

Here's an example for a TypeScript Vue app with Hot Reload:

```html
@using InertiaNetCore
@using InertiaNetCore.Utils

<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="utf-8"/>
    <meta name="viewport" content="width=device-width, initial-scale=1.0"/>
    <title inertia>My App</title>
</head>

<body>
@await Inertia.Html(Model)

@Vite.Input("src/app.ts")
</body>
</html>
```

And here is the corresponding `vite.config.js`

```js
import vue from '@vitejs/plugin-vue';
import { defineConfig } from 'vite';
import path from 'path';
import laravel from 'laravel-vite-plugin';
import { mkdirSync } from 'node:fs';

const outDir = '../../wwwroot/build';
mkdirSync(outDir, { recursive: true });

export default defineConfig({
    plugins: [
        laravel({
            input: ['src/app.ts', 'src/app.scss'],
            publicDirectory: outDir,
            refresh: true,
        }),
        vue({
            template: {
                transformAssetUrls: {
                    base: null,
                    includeAbsolute: false,
                },
            },
        }),
    ],
    resolve: {
        alias: {
            '@': path.resolve(__dirname, 'src'),
        },
    },
    build: {
        outDir,
        emptyOutDir: true,
    },
});
```
