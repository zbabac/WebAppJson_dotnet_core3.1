### This app demonstrates usage of .NET Core 3.1 handling POST (or GET) requests, parsing them (as JSON), and in this case, storing them into the MariaDB.  

**To be more interesting, it is used to parse the data sent from IoT LoraWAN server, with real data from sensors.**  

**Provided sample is fully functional, and it works from the Internet if you provide domain name to be used in nginx config.**  

You can only use .net part for your app, but if you want fully functional sample you need web server proxy and mysql. I used Nginx and MariaDB, but you may use Apache proxy and MySql server and configure it yourself. If you use my samples, the description how to configure is given below.  

- WebAPI is based on .NET Core 3.1, which is published on a Linux server with Nginx serving as a proxy. Security and https certificate and domain handling are left to the Nginx, so .net core 3.1 code is handled by the .net internal Kestrel server running on Linux localhost without worries about security.  
- It listens to the POST requests sent by a IoT LoraWAN network server, in which the sensor data updates are delivered.  
- It can be used for any WebAPI requests delivered in POST requests. The data then must be modified (or added new) in the SensorData class.  
- It demonstrates simple usage of Controllers and Models.  
- Controllers are the way the .net core 3.1 handle requests, in this case POST, in a way to simplify data extraction. This data is placed in a object (defined in a class), and then easily stored in database, or any other data store.  
- Models respresent objects that store the data. In fact in this way, the data definition is separated from the API. New objects can be added in a simple way and leave the main code unchanged.  
- The sample may be useful also for those exploring the land of LoraWAN IoT. In this sample, working LoraWAN network server sends sensor data updates. This WebAPI acts also as service that registers not just existing sensors data, but also adds new sensors into the DB.  
- I provided sample MySQL DB structure for this app. Sensor data (payload_hex) is not parsed as it differs from one sensor to another. It simple fetches the data and stores it into the DB.  
- DB structure for this app is in file sensors2.sql. The MariaDB server is installed on the same Linux machine where your app is run, as well Nginx.   
- JSON http POST (sent from Fiddler) request is stored in file fiddler_request_lorawan.txt, with the complete deserialized structure (in case you want to use it all). This is needed to simulate the real thing (I guess you don't have LoraWAN server at your disposal).  

### To summarize, you need a Linux server (I use cloud Debian 9 VM with 1cpu 1g ram), with a MariaDB, Nginx and .net core3.1 installed. Installation instructions for these 3 things can be easily found on Internet.  

I will explain how to deploy and setup .net core 3.1 as a service on that Linux box (file kestrel-webapi.service). The sample nginx config is given in file nginx.conf. The sample site config for nginx is given in file yoursite.com.conf (you will have to change to your site name). After installation you can simulate POST request from Fiddler or Postman (sample mentioned above).  

All development is done on Windows machine with Visual Studio 2019 Community edition. I used remote debugging (attach to the dotnet process on the Linux box and debug), so network connectivity between your development machine and Linux server is desirable.

I hope that this sample may be if use to someone, since I spent quite a time researching how does .net core 3.1 web api works, how to deploy to Linux and how to create a service that runs on nginx and persists on that Linux machine. In theory, there is no obstacle to use containers, i.e. docker to run .net core (with, of course MariaDB and nginx), since it is docker friendly, so I hope that someone will be able to give a working sample for docker.  

I use .NET Core 3.1 since it is LTS release and for this purpose can be easily deployed and run without issues on any newer Linux server. Just install prerequisites (MariaDB, Nginx and .netcore3.1).

In my sample the Web app is stored in `/var/www/webapi/` and nginx proxy redirects from _yourdomain_ to that app running on port 5001. So, I copy files from `.\bin\Debug` to the Linux `/var/www/webapi` (this is folder that holds my .net core app).
I configure the `nginx.conf` like in the sample provided (`/etc/nginx/`). I deviated slightly from default config and stored site data in `/etc/nginx/sites.d/` (there is my site named: `yoursite.com.conf`). You will put your file name and folders, but can retain the same structure. I reserved _https://yoursite.com/webapi_ for this webapp since I wanted to keep the main domain _https://yoursite.com_ for normal browsing. The LoraWAN server knows to send requests to the _https://yoursite.com/webapi_ and this address is not accessible via web browser, since it supports only POST requests (so, for testing you will need Fiddler or Postman). To have fully functional app you will need to install MariaDB and create sensors DB (sample file: `sensors2.sql`). Test the DB by running:  
`mysql -u root`  
`use sensors;`  
`desc sensors;`  

The application is simple and I suppose that you have basic knowledge of Linux, so I'll be short here. The app is complete and stable, and should run without issues if you configure nginx, mariadb and dotnet from the provided sample config files. Dotnet Kestrel web server runs on _localhost:5001_ and to be able to access it from the Internet, nginx is used as a proxy to redirect requests to _https://your_domain/webapi_ to the _https://localhost:5001_. If you are unfamiliar with nginx, leave everything as it is, just put your domain name in the new file `/etc/nginx/sites.d/yoursite.com.conf` config file (edit the file and put your domain instead yoursite.com). It should be easy and straightforward.  

To familiarize more, please check all provided config files (nginx, etc.), and read comments I placed there.     

After you copied binaries via e.g SFTP to `/var/www/webapi/`, configured nginx with new config files, and installed and configured MariaDB you have everything to test it. Start your dotnet app: `dotnet /var/www/webapi/WebAppJson.dll`, you should be able to see it as a process: `ps -ef|grep dotnet`. Don't forget to load new nginx config with: `nginx -s reload`, and try to send POST request from sample provided (fiddler_request_lorawan.txt).  
The result should be stored in DB (`select * from sensors; select * from history;`). Any errors could be seen in `/var/log/nginx/error.log`. If you experience errors, you can remote debug from VS (attach to process, select SSH and login to the Linux and then select dotnet process from the list).  

**After you have everything running and modify the app according to your need, you may want to have your Linux dotnet service to start automatically when your Linux starts and keep it running all the time. I explained how to create Linux service in comments in file: `kestrel-webapi.service`.**  

### So, to have it running, the flow is the following:  
 
 install nginx  
 install mariadb  
 install dotnetcore3.1  
  create dir `/var/www/webapi` (as normal user, you may need to change ownership later):  
 - `mkdir /var/www/webapi`  
 copy `.\bin\Debug\` files to the Linux: `/var/www/webapi/`  
 copy sensors2.sql to Linux and run:  
 - `mysql -u root < sensors2.sql`  
 edit provided nginx config files and use your domain name  
 copy provided nginx config files, create dir (as root):  
 - `mkdir /etc/nginx/sites.d/`  
 - copy `nginx.conf` to `/etc/nginx/`  
 - copy _yoursite.com_ (use your domain name here) to `/etc/nginx/sites.d/`  
 - reload nginx config: `nginx -s reload`  
 start app:  
 - `dotnet /var/www/webapi/WebAppJson.dll`  
 now everything is running, send simulated POST request from Fiddler or Postman!  