<#
.SYNOPSIS


.DESCRIPTION


.EXAMPLE


.INPUTS
None. You cannot pipe objects.

.OUTPUTS
None.

#>
param(
    [Parameter(Mandatory=$True, HelpMessage="Password for the PFX SSL certificate")]
    [string] $PASSWORD,

    [Parameter(Mandatory=$True, HelpMessage="API URL root")]
    [string] $REPLYURL
)

cd ..\Infra
terraform init
terraform apply -var pfx_password=$PASSWORD -var reply_url=$REPLYURL 
cd ..\Scripts