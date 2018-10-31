# Requirement
## Scenario
Serko Expense has a new requirement to import data from text received via email. The data will either be:

* Embedded as ‘islands’ of XML-like content
* Marked up using XML style opening and closing tags

The following text illustrates this:

Hi Victor,

Please create an expense claim for the below. Relevant details are marked up as requested:
**<expense><cost_centre>DEV002</cost_centre>
  <total>1024.01</total><payment_method>personal card</payment_method>
</expense>**

From: Jay Key 
Sent: Friday, 16 February 2018 10:32 AM 
To: Tony Spray 
Subject: test

Hi Tony,

Please create a reservation at the **&lt;vendor&gt;Viaduct Steakhouse&lt;/vendor&gt;** our **&lt;description>development team’s project end celebration dinner&lt;/description&gt;** on **&lt;date&gt;Tuesday 27 April 2017&lt;/date&gt;**. We expect to arrive around 7.15pm. Approximately 12 people but I’ll confirm exact numbers closer to the day.

Regards,
Jay

Your task is to write a REST service that:
* Accepts a block of text (assume it is plain text without non-English content, e.g. Germany, and XML tags have no duplications)
* Extracts the relevant data (all the XML data that are present in the text block)
* Calculate the GST and total excluding GST based on the extracted <total> (it includes GST)
* Makes the extracted and calculated data available to the service’s client (the calculated data and all the XML data that are present in the text block)

## Failure Conditions
The following failure conditions should be detected and made available to the client:
* Opening tags that have no corresponding closing tag. In this case the whole message should be rejected. 
* Missing &lt;total&gt;. In this case the whole message should be rejected. 
* Missing <cost_centre>. In this case the ‘cost centre’ field in the output should be defaulted to ‘UNKNOWN’.
