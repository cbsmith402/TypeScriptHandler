TypeScriptHandler
=================

Overview
--------

This is a simple HTTP handler module for IIS that allows you to directly reference your .ts TypeScript files in your HTML views. When the .ts is requested from the server, it will check to see if a .js version exists (or if the .ts has since been changed) and if not, it will compile the .ts into a .js. This way you don't have to worry about the disconnect between editing your .ts at design time and testing it at runtime.

Installing the module
---------------------
Prerequisites
* Node.js
* TypeScript (npm install -g typescript)
* .NET 4.0 framework (note that your IIS application pool also needs to run under .NET 4.0)
* Write permission by the application pool identity (usually IIS_IUSR) to the folder your .ts files are in

First, you'll need to clone this project and build it. This should produce TypeScriptHandler.dll.

Now, because npm installs global modules in %user%/AppData/Roaming for some odd reason, you'll need to put the typescript npm module and tsc.cmd somewhere that every process can access it. If you don't, you'll get some nasty permission errors when the IIS_IUSR process tries to execute something that belongs to the account you used to install TypeScript. The easiest way I found to do this is to copy the contents of %user%\AppData\Roaming\npm to %program files%\nodejs. Now you should be able to run "tsc" from a command line from any other user.

Once that's done, you'll need to install the TypeScriptHandler.dll in the /bin folder of your application. Then you can either add it as a handler through the IIS Manager or add the following web.config entry:

    <system.webServer>
        <handlers>
            <add name="TypeScriptHandler" path="*.ts" verb="*" type="TypeScriptHandler.Handler" resourceType="Unspecified" preCondition="integratedMode" />
        </handlers>
    <system.webServer>`

Using it
--------
To include a .ts file in your HTML, add a script reference as if it were a regular Javascript file:
    `<script type="text/javascript" src="/scripts/hello.ts" />`

Notice that the file extension is ".ts" instead of ".js".