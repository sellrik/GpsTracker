# GpsTracker
This Android application was created to provide a standalone gps tracking solution without any third party cloud service.
As of now the gps locations are sent via email.
The app only does the tracking part, to visualize, process the gps tracks, you can implement something like Csongor Varga created using Node-Red.
You can find his video here: https://www.youtube.com/watch?v=ywljyO74MjE


To keep the app running in the background you probably have to set some settings on your android device, check out this website for more details: https://dontkillmyapp.com/

Settings:
- Minimum time and distance: set the minimum time or distance interval between locations
- Telegram location upload: sends a Telegram message on each new location using the Telegram Bot api. The locations are not batched.
- Email sending: set the SMTP port, host, username, password, recipient and subject fields. The gps locations are sent in a attachment file (named data.json) in json format.
- Email sending interval (minutes): set the email sending interval. The minimum value is 15 minutes (this is the restriction of the used Android component)
- Keep location data for (days): the app keeps the logs for the specified days. The export function can be used to export the location logs in json or gpx format.
- Upload on mobile network
- Disable tracking when connected to WIFI

The apk is uploaded to the Release folder. To install it, you have to copy it to the device and enable unkown sources to install it.
