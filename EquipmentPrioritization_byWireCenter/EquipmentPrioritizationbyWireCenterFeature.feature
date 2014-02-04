Feature: EquipmentPrioritization_byWireCenter
			This app is a demonstration version of the Communications Edition apps available for purchase. It has been optimized for web use and is limited to Miami, FL.

             
                                     
 Background: 
  Given alteryx running at "http://gallery.alteryx.com/"
  And I am logged in using "deepak.manoharan@accionlabs.com" and "P@ssw0rd"
  
Scenario Outline: Equipment Prioritization by WireCenter
When I run analog store analysis with Average Number of Employees per Business on avgEmployee "<AvgEmployee>" avgBandWidth  "<AvgBandwidth>" voiceBandwidth "<VoiceBandwidth>" dataBandwidth "<DataBandwidth>"
Then I see the EquipmentPrioritizatiobyWireCenter result "<Result>"																			

Examples: 
| AvgEmployee     | AvgBandwidth | VoiceBandwidth | DataBandwidth | Result           |
| false           |      true    |     false      |      false    |1.37409755482732  |