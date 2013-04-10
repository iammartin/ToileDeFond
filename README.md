# ToileDeFond

ToileDeFond is a backend (.Net) modularity framework for web (mostly) or desktop applications, created and maintained by [Maxime Séguin](http://www.maximeseguin.com). The root idea was to build a CMS that gets out of your way by letting the developpers work as if there was no CMS while encouraging them to build small reusable modules.

####Important:
* This is mostly a pet project, not intended to be used in production. Use at your own risk!
* Some modules make use of third-party libraries ([RavenDB](http://ravendb.net/) for example), make sure you're compliant with all licenses


## Most notable features
* [ASP.NET MVC](http://www.asp.net/mvc) Modularity (with the help of [MEF](http://mef.codeplex.com/))
* Content Management (with the help of [RavenDB](http://ravendb.net/)... could be implemented with another persistence tool)
  * Frontend scaffolding ([ASP.NET MVC](http://www.asp.net/mvc) Based)
* Simple Membership (rewriten to work with [RavenDB](http://ravendb.net/) - only a proof of concept at this time)


## Authors

**Maxime Séguin**

+ [http://twitter.com/w3max](http://twitter.com/w3max)
+ [http://github.com/w3max](http://github.com/w3max)
+ [http://www.maximeseguin.com](www.maximeseguin.com)


## Copyright and license

Copyright 2013 Maxime Séguin

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this work except in compliance with the License.
You may obtain a copy of the License in the LICENSE file, or at:

  [http://www.apache.org/licenses/LICENSE-2.0](http://www.apache.org/licenses/LICENSE-2.0)

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
