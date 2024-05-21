# vr-file-browser
Browse your entire computer in VR and filter out the content you want. The code is rough, but works.
I used TMP_Pro, so it's just one canvas and the beam, based on collider.

You can filter any extensions, just read the scripts I wrote. I tried to make it as understandable as possible for beginners.


The beam must be parented to a dummy object under the headset or controller. It uses some smoothing, so you don't need to worry of shaky hands.


Everything is based on few images and some scripting. I'm using this with my VR AI HoloPlayer. Thought to release as this kind of thing in VR seems to be hard to find.


If you want to link the beam clicks to the controller, again, read the scripts. You might wanna scale the beam in Z to make the collision.


One thing I forgot to do was to change the prefab canvas to Default layer. It's a must.
