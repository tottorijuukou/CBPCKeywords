# BookUUNPCBPCKeywords

This is a Skyrim Mutagen patcher for adding CBPC armor keywords to ARMO records so you don't have to do it in SSEEdit.

## What are CBPC keywords and why do I care?

There are essentially four types of chest armor (or as this document will refer to them, just plain 'armor') in Skyrim:  
* clothing
* light armor
* heavy armor
* naked (i.e. nothing/the actor's default skin armor)

In CBPC's armor config files, each of these four types can be given two parameters:
* pushup  - how much it will raise the base position of breasts while animating
* amplitude  - how much it will turn down (or up) the jiggle

By default CBPC will just check the type of equipped armor and act on its configurations from there.

However, the latest version of CBPC (1.5 at the time of this writing) introduced the ability to check the armor for certain KYWDs that act as overrides to its behavior.  A heavy armor set can be told to animate as clothes, for instance.

## Some config settings you can either change to your own preferences or just complain about

This patcher presumes you are going to do something like this with all the armor keywords it will add for you:

* Naked is "no changes to the base settings with this armor".
* Clothed is adding some pushup, and dialing down the jiggle some.
* Light is adding full pushup, and dialing down most of the jiggle.
* Heavy is adding full pushup, and zeroing out the jiggle.

## How does I immerse physics?

Heretofore the only way to get jiggly armor or clothes in an 'immersive' (i.e. appearing to vaguely respect the structural integrity of meshes textured like metal plates/thick leather etc etc etc) sort of way was to completely disable any physics capabilty in something you didn't want wobbling around (the classic 'physics' vs 'no physics'  bodyslide method).  However, a comprehensive use of CBPC keywords lets you do things that weren't possible before, like:

* make some clothes and light armors less (or more) wobbly/solid.  
* make steel bikinis act like bikinis without affecting all other heavy armor pieces
* disable pushups to avoid localized instance of weird antigravity
* give different settings to each breast  (ok, not a lot of meshes are going to get much use from this, but it's *possible* at least)

# Actually running this thing

This patcher will look for the CBPC keywords and use those if you're already using them so as not to gunk up your records with duplicate keys.  And if you don't, it will just add them to the synthesis patch (these 10 records really shouldn't affect your ability to ESLify it later unless you are already doing some VERY EXTREME PATCHING).  For modularity purposes it will read all the .json files in its internal data folder (this will end up in your Synthesis folder under Data/Skyrim Special Edition/BookUUNPCBPCKeywords if you do the presumed install from git thing) so you can just make your own new file if you want to add some things without wading through a bunch of previously existing .json junk.  The format for these files is pretty dirt simple:

```
{
	"ThisIsTheARMOEditorID": [ "TheseCBPCKeywords", "GetAppliedTo", "YourSynthesisPatch" ]
}
```

If you don't like my pre-built classifications, you can either find the records in the .json files and edit them or use the special file override.json, which is read last and is the only file that will override anything from the other .json files (otherwise it's first come first served).  bookuunp.json is the base file with the vanilla and Book of UUNP records and you can't rename or replace it or the patcher will fail and I will point to this part where I told you not to mess with it.

## Supported out of the box

You might have noticed by the title that this thing was designed for at least one mod in mind.  Anyway, here are the mods with supported editorID records:

* the base game (with enchanted variants)
* Book of UUNP
* Common Clothes and Armors (most of them)
* BD's Armor and Clothes Replacer (the standalone versions)

## Other notes on running

This patcher only operates on ARMOs, so in the event you are doing multiple synthesis runs due to whatever limitation, run it with the group that edits ARMOs.