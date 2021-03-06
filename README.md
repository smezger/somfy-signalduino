# somfy-signalduino
## Logfile sample
Set_sendMsg, msg=P43#A38B8B786D7566#R6
2020.09.20 07:56:37 5: sduino: Set_sendMsg, Preparing manchester protocol=43, repeats=0, clock=645 data=A38B8B786D7566

## Explanation
Protcol 43 = Somfy RTS
C = Clock = Frequenz
F= Register CC1101
2020.09.20 07:56:37 5: sduino: AddSendQueue, sduino: SC;R=6;SR;P0=-2560;P1=2560;P3=-640;D=10101010101010113;SM;C=645;D=A38B8B786D7566;F=10AB85550A;

SC = Send Combined -> SR (Send Raw) und SM( Send Manchester) abwechselnd

R = Repeat

für Raw wird mit PYX die Pulse definiert sprich P0 = für den 0 char etc

das scheint dann die Zeit in ys zu sein und - für 0 und + für 1.

D.h. laut https://pushstack.wordpress.com/somfy-rts-protocol/ verwenden sie die art für wiederholende frames mit 7 mal 10 pattern

und dann soft sync

das entspricht dem Dta des Send Raw

10
10
10
10
10
10
10
113

Protocol Data ist hier drin also P43 = Somfy RTS
https://github.com/fhem/fhem-mirror/blob/master/fhem/FHEM/lib/SD_ProtocolData.pm
Address 131815

enc key A1

rolling code 0101

key up


2020.09.24 15:39:30 5: sduino: Set_sendMsg, msg=P43#A2848587928A99#R6

2020.09.24 15:39:30 5: sduino: Set_sendMsg, Preparing manchester protocol=43, repeats=0, clock=645 data=A2848587928A99

2020.09.24 15:39:30 5: sduino: AddSendQueue, sduino: SC;R=6;SR;P0=-2560;P1=2560;P3=-640;D=10101010101010113;SM;C=645;D=A2848587928A99;F=10AB85550A; (1)

2020.09.24 15:39:30 4: sduino: Set_sendMsg, sending : SC;R=6;SR;P0=-2
## New example
* RC: 108
* EC: A8
<pre>
020.10.05 09:38:20 4: SOMFY_set: Light -> entering with mode :send: cmd :off:  arg1 ::  pos :200: 
2020.10.05 09:38:20 4: SOMFY_set: handled command off --> move :off:  newState :open: 
2020.10.05 09:38:20 5: SOMFY_set: handled for drive/udpate:  updateState ::  drivet :0: updatet :0: 
2020.10.05 09:38:20 4: SOMFY_sendCommand: Light -> cmd :off: 
2020.10.05 09:38:20 4: SOMFY_SENDsm message: sA8200108131815
2020.10.05 09:38:20 4: SOMFY_SENDsm data before checksum: sA8200108131815
2020.10.05 09:38:20 4: SOMFY_SENDsm data checksum: 6
2020.10.05 09:38:20 4: SOMFY_SENDsm data after crypt: A88E8F87928A99
2020.10.05 09:38:20 5: sduino: Write, sending via Set sendMsg P43#A88E8F87928A99#R6
2020.10.05 09:38:20 5: sduino: Set_sendMsg, msg=P43#A88E8F87928A99#R6
2020.10.05 09:38:20 5: sduino: Set_sendMsg, Preparing manchester protocol=43, repeats=0, clock=645 data=A88E8F87928A99
</pre>
