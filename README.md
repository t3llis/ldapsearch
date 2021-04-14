# ldapsearch
A C# implementation of LDAP over SSL (LDAPS) search utility

Currently only DirectorySearcher.FindAll method is supported, meaning that all entries are returned for the specified LDAP filter.

Useful for situations where encrypted interaction with LDAP server is required, i.e. exchanging credentials over an encrypted network or avoiding on-premises logging platforms during Simulated Attacks such as Advanced Threat Analytics (ATA).

It can also be used with modern Command & Control (C2) frameworks supporting fork & run such as Cobalt Strike.

Further functionality is to be developed such as adding new LDAP entries etc. Nevertheless, feel free to contribute to the project.
