using System.Reflection;
using System.Runtime.InteropServices;

// Les informations générales relatives à un assembly dépendent de 
// l'ensemble d'attributs suivant. Changez les valeurs de ces attributs pour modifier les informations
// associées à un assembly.
[assembly: AssemblyTitle("smartTracker 2014")]
[assembly: AssemblyDescription("RFID SPACECODE Managment software")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("SPACECODE")]
[assembly: AssemblyProduct("smartTracker 2014")]
[assembly: AssemblyCopyright("Copyright ©  2014")]
[assembly: AssemblyTrademark("SPACECODE")]
[assembly: AssemblyCulture("")]

// L'affectation de la valeur false à ComVisible rend les types invisibles dans cet assembly 
// aux composants COM. Si vous devez accéder à un type dans cet assembly à partir de 
// COM, affectez la valeur true à l'attribut ComVisible sur ce type.
[assembly: ComVisible(false)]

// Le GUID suivant est pour l'ID de la typelib si ce projet est exposé à COM
[assembly: Guid("dfe15278-5765-4741-b986-eb5ea62058b0")]

// Les informations de version pour un assembly se composent des quatre valeurs suivantes :
//
//      Version principale
//      Version secondaire 
//      Numéro de build
//      Révision
//
// Vous pouvez spécifier toutes les valeurs ou indiquer les numéros de build et de révision par défaut 
// en utilisant '*', comme indiqué ci-dessous :
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("2021.1.20.1698")]
[assembly: AssemblyFileVersion("2021.1.20.1698")]
//1696 - bug tag to drawer ARM get inventories
//1694 - Run one scan after connect for LED 
//1684 - badge reader remote on SMC
//1680 : test XU4 temperature kirsh
//1676 Add unlock comand with TCP (Betrace) 
// Need to be allowed in device application with a crypted authorization string in App.config
//2015.6.3.1674 - Change for testRFID in Windows 10 not compatible due to decorated port name
//1670 - Change server for cabinet fridge
//collision btw badge and TCP scan
// user  info lost
//1666 Add JackPot
// Bug convertfor store if null
//1664 Led for Pad
//     Arm Tcp server port changeable
//1662 Light SPCE2_RO uid  
//1660 Scale port Exception
//1658 Server Only KSA test (Remove later)
// 1656 Fix Bug connection Scale with SmartPad
// Add TCP query to renew Fingerprint 
// System.DiposedObject on waitHandle tested
//1652 Bug Date time with summer time schroff
// Change scale port com detection (bug W8) and smartPad
//1650 Block FP of SAS if fp touch
//1644 Fixed Bug Connection unknown command
       //Tcp Arm socket change
//1640 Change for server Mode for SAS
//     Avoid opening of the door if a user is authenticate so if is a door unlocked
//1638  SBR V3 Disconnection/connection 
//      remove usb watcher
//      Change serial port detection
//1636
// Fix Bug TCParm device tcp without inventory in the local database
//1634 CVS for JSC eurostar
// 1634 TCP for ARM SBR DEVICE

// 1630 Null ref Exception Server STATUSANDTAGS
// Bug scan in process not hiding while completed

// 1628 - bug shelve fridge
//      - Add temp notification        

//1624 - add systray for server Eurostar
//move cvs init

// 1622 - New SDK For GIA with writing decimal
// new tag notification for device 3.x

//1620 - Light live data for device

//1618 : Light OFF for LED feature in device

// 1616 - Change message box message for LED to close it automatically for door devcie
// 1614 : Increase speed for unlock for LED // TO TEST WITH XP and SBR with PC

//1612 Renew FP with autorescan fo VK468
// Add a schedule task with devcon and removeFP2.bat

// 1610 - Detection device autofill if several device other that SBR.
// time door close shroff cabinet


//1608 - new search tag function
// Ambiant Temp in schroff
// LEd Box Mode
//1604 : T° Display fridge modification
//     : Evermed T° 
//     medical schroff  time zone hour
// 1602 :
// bug Case sensitive product reference

// Increase speed tcp light version

//1598 - add fanem fridge device
//1596 add export CSV for eurostar
// Change alpha code SPE2
//1594 - Change SPCE2 Table Value
// ADD 2 notification for SPCE2 (RO/RW)
// Modify writing process generation new UID with new Table


//1590 Bug GroupMode excape char' in sql
//1588 Put emergency opening for medical device only
//1584
// Bug MSR log file not accesible
// Bug ComBadge on windows 8
// Bug selection index higher than Local device

//1580
// Ecosafe Cabinet
// Froster Temperature
//1578
//Invert slave door / master door while display emergency opening bad door
// BUG UID CONTAINING §
//1576 
// Bug USB with using in watcher
// Bug Ethernet vesrion if USer request two scan before the client request it.
// Temperature change and history

//1574 Change img tab in datalist
// bug led one by one stop thread

//1572 Get network scan by ID (avoid bug gmt date if device not )
//1570 WRITE SPCE2 With EOF support (Speed increase)
// Tag UID history
// SAS error Mergency opening

//1566 Add user name unknown while touch FP
// Bug confirmation write (bad choice axis)

//1564 Accelerate end scan for SBR networked

//1562 - Change WMI check fdti dual port to remove admin right
// remove oleDB export feature - export only in 2007 2013 excel

//1560 - use new lib excel.dll to import.
// Remove provider access and bitness

//1558 - Add Copy yapi.dll + instal passive access provider 
//Remove bitness version

//1556 - Add resolve assembly for sqlite choice
// bug fix when failed start scan resuest on door open
// Add device SMC for cathrack led lighting on badge read

//1554 - bug test lighting axis
//1544 - Change fridge connection for temp recovery
// 1540 - Change medicalCabinet Class For cathrack 
// Ie old stentbox schroff are now cancelled

// 1538 - Add temperarture history in ST2013
// 1536 - Add SBF BL 520 LED

//28 10 2013 - 3 - 1530
//add SPCE2 LED AND WRITING CAPABILITY
// Show identity card for comparison
//Bug fixes end of scan
// Remove messagebox after set IP


//Bug fixes (scan stopping at stratup

// 24 09 2013 - 1528
// Add Compatibility in readonly for SPCE2/R8

//19 09 2013 - 1526
// Reset clock timer interval for Light flashing when user not open door

//13 09 2013 - 1524
//  LED flashing when user authentified

//13 09 2013 - 1522
// Block access of door during process data

// 13 09 2013 - 1520
// Change LEd process on axis by axis.

// 05 09 2013 - 1518
// Remove watchdog REnewFP for liebher fridge for SAS slave disconnect
//04 09 2013 - 1514
// Fix bug MSR if multiple MSR for renew FP
// Bug SAS in renewFP
// SpareData in notification with badgeID

// 05 08 2013 - 2013.6.2.1512
// Bug VS 2012 - Crreate Chart As designer failed
// Caution V1508 is corrupt for Scale

// 26 07 2013 - 2013.6.2.1506
// Remove grant on startup
// Check admin right for scale and local device
// temperature bottle and chamber

// 13 07 2013 - 2013.6.2.1500
// Add LED Support for SPCE2
// Add GetFullPath for import relative Path
// Add Automatic detection for Scale 
// Thread notification

// 09 07 2013 - 2013.6.2.1490
// add TCP notification for device
// remove rebuild column in datalist
// Bug stop scan in accumulate
// IdscanEvent pout id de scan unique

// 27 06 2013 - 2013.6.2.1484
// add chart for scale.
// change gui for scale

// 17 06 2013 - 2013.6.2.1482
//  add jump in next empty lotID in autofill.
//  Remove limitation box mode
//  Bug compare inventory

// 10 06 2013 - 2013.6.2.1478
// Add char ' / () in column name.
// Change reserved column name status and location to ImportStatus and TagLocation
// To  let user able to take this name.
 
// 27 05 2013 - 2013.6.2.1476
// Change tcpServer - Add sql export feature. 
// 23 05 2013 - 2013.6.2.1474
// Add application setup parameter to stop scan or not if user press fingerprint during scan.

// 21 05 2013 - 2013.6.2.1472
// Issue doorOpentToolong if not define in DB 

// 15 04 2013 - 2013.6.2.1468
// Add automatic weight scale feature

// 10 04 2013 - 2013.6.2.1464
// add support sartorius wait scale to update carats weight.

// 21 03 2013 - 2013.6.1.1462
// Bug REfresh wait Mode

// 21 03 2013 - 2013.6.1.1460
// renew Fp every 6 hours


// 12 03 2013 - 2013.6.1.1458
// Add remove FP as Failure after xhours on few PC
// Add Direct TCP Command for SBR 


// 07/03/2013 - 2013.6.1.1454
// Fix Issue Last user keep grant even if deleted

// 19 02 2013 - 2013.6.1.1452
// Fix Bug Box Mode UID verification

// 14 02 2013 - 2013.6.1.1450
// Auto update for device

// 04 02 2013 - 2013.6.1.1448
// Add TCP/IP server direct Mode
// Change finish accumulate

// 01 02 2013 - 2013.6.1.1444
// Add fingerprint reader for SFR

// 23 01 2013 : 1442
// Fix Bug save door grant in SQL server

// 18 01 2013 : 1440
// Stop progress scan for USD device (Paul request)



// 03 01 2012 : 2013.6.1.1438
// add miniSAS in device detection by return classic serial port
// if no ftdi is plugged

// 21 12 2012 : 2013.6.1.1434
// accumulate with box tag
// Fix update update history
// Accumulate with box 


//18 12 2012 : 2013.6.1.1432
// Change fix bug when choose reader history
// add enumeration description
// SAS double door user enable feature

//17 12 2012 : 2013.6.1.1428
// Fix Accumulation with new end scan test

// 06 12 2012 : 2013.6.1.1424
// Fix bug column TagUID if not named TagUID
// Feature - Close live data in box criteria and autofill item list if user need reader.
// Display hotlink with max 10 column name in any case
// Fix - Removed item not take into account in criteria check
// Fix - Box tag not displayed again due to previous change in inventory process (v 1416)

// 04 12 2012 : 2013.6.1.1422
// Fix Compare inventory 
// Fix Stop scan with the animation 

// 04 12 2012 : 2013.6.1.1416
// Fix Bug tag added not processes that show bad number of tag
// Fix bug refresh compteur

//29 11 2012 : 2013.6.1.1408
// Change Tcp /ip with DB/noDB  function to fit for SDK

// 15 11 2012 - 2013.6.1.1408
// add set ip in Tcp server.
// add detect usb key and launch .bat to reset IP valuee in case

// 15 11 2012 - 2013.6.1.1404
// add sql server database (same feature as sqlite fornow)
// add viewer mode

// 07 11 2012 - 2013.6.0.1400
// Limit stock alert for smc
// fixed bugs autofill in item list with RFID
// stop exception sending mail if no internet
// check in function convert for store
// check in function convert for use

// 06 11 2012 - 2013.6.0.1396
// check bad device configuration to allow close live data
// change com port detection on ftdi device to allow bluetooth port and modem

// 05 11 2012 - 2013.6.0.1394
// Bug blinking display 

// 31 10 2012 - 2013.6.0.1392
// Add blood fridge device
// alert Blood patient
// alert stock low

// 03 10 2012 - 2013.6.0.1390
// Notification end of scan
// 3 months licence by default 
// Groups collapsed 
// Item list with search
// Exportation in standard Mode
// Add formula feature
// Add load image in item list by lotID
// Color In Item History
// Compare 2 inventories
// shortcut F3 stop scan , F4 keep last scan
// Dynamic launch an excel Macro from an inventory result
// Reader history to load (keep previous scan)/display previous scan

// 12 09 2013
// change for MSR

//07 09 2013 - 2013.5.0.1380
// add refresh user TCP/IP

//09 08 2012 - 2013.5.0.1376
// Add mini SAS Device

//27 07 2012 - 2013.5.0.1370
//add licence info user company machine name
// add several machine for alarm remove time
//24 07 2012 - 2012.5.0.1359
// Add listview in compare inventory

//18 05 2012 - 2012.5.0.1353
//Bug Refresh inventory

//18 05 2012 : 2012.5.0.1351
// Add history in lotID
// Add alarme for TCP/IP device
// Bug overwrite

// 17 05 2012 : 2012.5.0.1349
// Passage objeclistview
// Add network alarm
// remove trial version (need to be change)

// 04 07 2012 : 2012.3.4.1347
// reduce High CPU usage in XP SP3 with WMI USB watcher queries
// Test for refresh tabControl if not in scan

// 02 07 2012 : 2012.3.4.1345
// Change connection string for excel 2003 bug at export.
// add copy to clipboard lotID feature
// add fridge support without t°
// All tread in backgroung mode

//25 06 2012 : 2012.3.4.1343
// Correction Bug in compare inventory when compare one tagID
// with unreferenced tag

// 22 06 2012 : 2012.3.4.1341
// add auto load feature in item list with RFID support for jewelry

// 22 06 2012 : 2012.3.4.1339
// Change update datagridview at end of scan.
// Export sum Value if present

//19 06 2012 : 2012.3.4.1337
// Increase Speed importation and add some status info + Check UID format and length
// Accept until 50 columns
// Press F2 to launch scan of selected ready reader
// Change Configuration application window with decription of each property
// display sum of value items if define in column database as AT_Limit_Value_Exceed
// Exportation compare inventory depending display (all or missing)
// Bug display grid in accumulate mode for SBR
// Automatic restart after column or configuration changed
// Add image from file and from library (already in DB)

//13 06 2012 : 1012.3.3.1335
// Press F2 to launch scan of selected ready reader

// 05 06 2012 : 2012.3.3.1333
// Add tcp/IP real time view

// 31 05 2012 : 2012.3.3.1329
// Image  can be reused after first use

// 23 05 2012 ; 2012.3.3.1327
// Add change in ethernet support version
// add in/out mode for SDR USB

// 10 05 2012 : 2012.3.3.1325
// Add up to 30 columns available for product database
// Add import header columns from excel file
// Add default configutation for alert ,smtp 

// 7 05 2012 : 2012.3.3.1317
// Change Manual name scan
// add move sensor alert

// 4 05 2012 : 2012.3.3.1315
// Add alert/mail feature

// 27 04 2012 : 2012.3.3.1311
// Add Group module
// change some GUI apect
// Business Report Activity + Compare between Date


// 16 04 2012 : 2012.3.3.1307
// Add ping dns infomaniak for error during  get reader info
// Add image delete windows, feature.
// Move add image feature to item list windows
// Add auto association in item list



//11 04 2012 : 2012.3.3.1305
// Change connection for several device not conected KB
// Add image support
// Add color


// 26 03 2012 : 2012.3.3.1303
// Add Licencing feature with a 30 days 30 times trial mode.
// Collapse node + Color in treview device

//22 03 2012 : 2012.3.1295
// Add change DB column dynamically
// New ICONE
// Screen Saver

//19 03 2012 : 2012.3.2.1291
// Bug in Item List while use one get without import

//15 03 2012 : 2012.3.2.1272
// Bug on Bitness Windows detection while force compiler to X86 on a 64 bits OS

// 14 03 2012 : 2012.3.2.1269
// Minor version change to 20.12.3.2
// Add dynamic colum number info (up to ten)
// Add import and compare CSV with overwrite function
// Add Delete association
// Add Keep last scan in accumulate mode

// 08 03 2012 : 2012.3.1.1267
// Change Framework from 2.0 to 3.5 to use EEPlus Library
// Allow exporting to excel 2007 and 2010 with formating cell and adjust column
// Allow importing by picking 1 sheet name (KB issue with oleDB that cannot get first sheet but alphabatical one)

// 06 03 2012 : 2012.3.0.1264
// Accept double in import
// Remove empty cell from import and compare inventory


// 02 03 2012 : 2013.3.0.1262
// Correct Bug in User Managment on Storing Grant

// 01 03 2012 : 2012.3.0.1260
// Add update color in compare inventory after sorting by header click

// 29 12 2012 : 2012.3.0.1258
// Add lotid and tag id compare choice (Or / And)
// Change installation check Bitness by outlook 2010 x 64 presence

// 28 02 2012 : 2012.3.0.1256 
// Add  CompareByLotID Feature

// 22 02 2012 : 2012.3.0.1240
// Change Calibration Graph with try catch to old one in case of failure of draw curve
// Change light managment 

// 17 02 2012 : 2012.3.0:.235
// Add Ctrl D to access to debug  
// Limit F choice
// Bug chgt Reference to LotID 

// 15 02 2012 : 2012.3.0.1167
// Add Comparaison inventory feature

// 14 02 2012 : 2012.3.0.1096
// Add configuration menu
// Export Inventory With template