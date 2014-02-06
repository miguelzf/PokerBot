using System;using Tools;
#pragma warning disable 1591
namespace YYClass {
//%+GStuff+16
/// <exclude/>
public class GStuff : TOKEN{
/// <exclude/>
public override string yyname { get { return "GStuff"; }}
/// <exclude/>
public override int yynum { get { return 16; }}
/// <exclude/>
public GStuff(Parser yyq):base(yyq){ }}
//%+Stuff+17
/// <exclude/>
public class Stuff : TOKEN{
/// <exclude/>
public override string yyname { get { return "Stuff"; }}
/// <exclude/>
public override int yynum { get { return 17; }}
/// <exclude/>
public Stuff(Parser yyq):base(yyq){ }}
//%+Item+18
/// <exclude/>
public class Item : TOKEN{
/// <exclude/>
public override string yyname { get { return "Item"; }}
/// <exclude/>
public override int yynum { get { return 18; }}
/// <exclude/>
public Item(Parser yyq):base(yyq){ }}
/// <exclude/>
//%+ClassBody+19
public class ClassBody : TOKEN{
/// <exclude/>
public override string yyname { get { return "ClassBody"; }}
/// <exclude/>
public override int yynum { get { return 19; }}
/// <exclude/>
public ClassBody(Parser yyq):base(yyq){ }}
//%+Cons+20
/// <exclude/>
public class Cons : TOKEN{
/// <exclude/>
public override string yyname { get { return "Cons"; }}
/// <exclude/>
public override int yynum { get { return 20; }}
/// <exclude/>
public Cons(Parser yyq):base(yyq){ }}
//%+Call+21
/// <exclude/>
public class Call : TOKEN{
/// <exclude/>
public override string yyname { get { return "Call"; }}
/// <exclude/>
public override int yynum { get { return 21; }}
/// <exclude/>
public Call(Parser yyq):base(yyq){ }}
//%+BaseCall+22
/// <exclude/>
public class BaseCall : TOKEN{
/// <exclude/>
public override string yyname { get { return "BaseCall"; }}
/// <exclude/>
public override int yynum { get { return 22; }}
/// <exclude/>
public BaseCall(Parser yyq):base(yyq){ }}
//%+Name+23
/// <exclude/>
public class Name : TOKEN{
/// <exclude/>
public override string yyname { get { return "Name"; }}
/// <exclude/>
public override int yynum { get { return 23; }}
/// <exclude/>
public Name(Parser yyq):base(yyq){ }}
/// <exclude/>
public class ClassBody_1 : ClassBody {
  /// <exclude/>
  public ClassBody_1(Parser yyq):base(yyq){}}
  /// <exclude/>
public class ClassBody_2 : ClassBody {
  /// <exclude/>
  public ClassBody_2(Parser yyq):base(yyq){}}

/// <exclude/>
public class ClassBody_2_1 : ClassBody_2 {
  /// <exclude/>
  public ClassBody_2_1(Parser yyq):base(yyq){ yytext=
	((GStuff)(yyq.StackAt(1).m_value))
	.yytext; }}
/// <exclude/>
public class GStuff_1 : GStuff {
  /// <exclude/>
  public GStuff_1(Parser yyq):base(yyq){}}
/// <exclude/>
public class GStuff_2 : GStuff {
  public GStuff_2(Parser yyq):base(yyq){}}

  /// <exclude/>
public class GStuff_2_1 : GStuff_2 {
  public GStuff_2_1(Parser yyq):base(yyq){ yytext=""; }}
  /// <exclude/>
public class GStuff_3 : GStuff {
  public GStuff_3(Parser yyq):base(yyq){}}
  /// <exclude/>
public class GStuff_4 : GStuff {
  public GStuff_4(Parser yyq):base(yyq){}}
  /// <exclude/>
public class GStuff_4_1 : GStuff_4 {
  public GStuff_4_1(Parser yyq):base(yyq){ yytext=
	((GStuff)(yyq.StackAt(1).m_value))
	.yytext+
	((Cons)(yyq.StackAt(0).m_value))
	.yytext; }}
    /// <exclude/>
public class GStuff_5 : GStuff {
  public GStuff_5(Parser yyq):base(yyq){}}
  /// <exclude/>
public class GStuff_6 : GStuff {
  public GStuff_6(Parser yyq):base(yyq){}}
  /// <exclude/>
public class GStuff_6_1 : GStuff_6 {
  public GStuff_6_1(Parser yyq):base(yyq){ yytext=
	((GStuff)(yyq.StackAt(1).m_value))
	.yytext+
	((Item)(yyq.StackAt(0).m_value))
	.yytext; }}
    /// <exclude/>
public class Stuff_1 : Stuff {
  public Stuff_1(Parser yyq):base(yyq){}}
  /// <exclude/>
public class Stuff_2 : Stuff {
  public Stuff_2(Parser yyq):base(yyq){}}
  /// <exclude/>
public class Stuff_2_1 : Stuff_2 {
  public Stuff_2_1(Parser yyq):base(yyq){ yytext=""; }}
  /// <exclude/>
public class Stuff_3 : Stuff {
  public Stuff_3(Parser yyq):base(yyq){}}
  /// <exclude/>
public class Stuff_4 : Stuff {
  public Stuff_4(Parser yyq):base(yyq){}}
  /// <exclude/>
public class Stuff_4_1 : Stuff_4 {
  public Stuff_4_1(Parser yyq):base(yyq){ yytext=
	((Stuff)(yyq.StackAt(1).m_value))
	.yytext+
	((Item)(yyq.StackAt(0).m_value))
	.yytext; }}
    /// <exclude/>
public class Cons_1 : Cons {
  public Cons_1(Parser yyq):base(yyq){}}
  /// <exclude/>
public class Cons_2 : Cons {
  public Cons_2(Parser yyq):base(yyq){}}
  /// <exclude/>
public class Cons_2_1 : Cons_2 {
  public Cons_2_1(Parser yyq):base(yyq){ 
			cs0syntax yy = (cs0syntax)yyq;
			if (
	((Name)(yyq.StackAt(4).m_value))
	.yytext.Trim()!=yy.Cls)
					yytext=
	((Name)(yyq.StackAt(4).m_value))
	.yytext+"("+
	((Stuff)(yyq.StackAt(2).m_value))
	.yytext+")";
			else {
				if (
	((Stuff)(yyq.StackAt(2).m_value))
	.yytext.Length==0) {
					yytext=
	((Name)(yyq.StackAt(4).m_value))
	.yytext+"("+yy.Ctx+")"; yy.defconseen=true;
				} else
					yytext=
	((Name)(yyq.StackAt(4).m_value))
	.yytext+"("+yy.Ctx+","+
	((Stuff)(yyq.StackAt(2).m_value))
	.yytext+")"; 
				if (
	((BaseCall)(yyq.StackAt(0).m_value))
	.yytext.Length==0)
					yytext+=":base("+yy.Par+")";
				else
					yytext+=":"+
	((BaseCall)(yyq.StackAt(0).m_value))
	.yytext.Substring(0,4)+"("+yy.Par+","+
	((BaseCall)(yyq.StackAt(0).m_value))
	.yytext.Substring(4)+")";
				}
			}}
            /// <exclude/>
public class Call_1 : Call {
  public Call_1(Parser yyq):base(yyq){}}
  /// <exclude/>
public class Call_2 : Call {
  public Call_2(Parser yyq):base(yyq){}}
  /// <exclude/>
public class Call_2_1 : Call_2 {
  public Call_2_1(Parser yyq):base(yyq){ 
			if (
	((Name)(yyq.StackAt(3).m_value))
	.yytext.Trim()!=((cs0syntax)yyq).Cls)
					yytext=
	((Name)(yyq.StackAt(3).m_value))
	.yytext+"("+
	((Stuff)(yyq.StackAt(1).m_value))
	.yytext+")";
			else {
				if (
	((Stuff)(yyq.StackAt(1).m_value))
	.yytext.Length==0)
					yytext=
	((Name)(yyq.StackAt(3).m_value))
	.yytext+"("+((cs0syntax)yyq).Par+")";
				else
					yytext=
	((Name)(yyq.StackAt(3).m_value))
	.yytext+"("+((cs0syntax)yyq).Par+","+
	((Stuff)(yyq.StackAt(1).m_value))
	.yytext+")"; 
				}
			}}
            /// <exclude/>
public class BaseCall_1 : BaseCall {
  public BaseCall_1(Parser yyq):base(yyq){}}
  /// <exclude/>
public class BaseCall_2 : BaseCall {
  public BaseCall_2(Parser yyq):base(yyq){}}
  /// <exclude/>
public class BaseCall_2_1 : BaseCall_2 {
  public BaseCall_2_1(Parser yyq):base(yyq){ yytext=""; }}
  /// <exclude/>
public class BaseCall_3 : BaseCall {
  public BaseCall_3(Parser yyq):base(yyq){}}
  /// <exclude/>
public class BaseCall_4 : BaseCall {
  public BaseCall_4(Parser yyq):base(yyq){}}
  /// <exclude/>
public class BaseCall_4_1 : BaseCall_4 {
  public BaseCall_4_1(Parser yyq):base(yyq){ yytext="base"+
	((Stuff)(yyq.StackAt(1).m_value))
	.yytext; }}
    /// <exclude/>
public class BaseCall_5 : BaseCall {
  public BaseCall_5(Parser yyq):base(yyq){}}
  /// <exclude/>
public class BaseCall_6 : BaseCall {
  public BaseCall_6(Parser yyq):base(yyq){}}
  /// <exclude/>
public class BaseCall_6_1 : BaseCall_6 {
  public BaseCall_6_1(Parser yyq):base(yyq){ yytext="this"+
	((Stuff)(yyq.StackAt(1).m_value))
	.yytext; }}
    /// <exclude/>
public class Name_1 : Name {
  public Name_1(Parser yyq):base(yyq){}}
  /// <exclude/>
public class Name_2 : Name {
  public Name_2(Parser yyq):base(yyq){}}
  /// <exclude/>
public class Name_2_1 : Name_2 {
  public Name_2_1(Parser yyq):base(yyq){ yytext=" "+
	((ID)(yyq.StackAt(0).m_value))
	.yytext+" "; }}
    /// <exclude/>
public class Name_3 : Name {
  public Name_3(Parser yyq):base(yyq){}}
  /// <exclude/>
public class Name_4 : Name {
  public Name_4(Parser yyq):base(yyq){}}
  /// <exclude/>
public class Name_4_1 : Name_4 {
  public Name_4_1(Parser yyq):base(yyq){ yytext=
	((ID)(yyq.StackAt(3).m_value))
	.yytext+"["+
	((Stuff)(yyq.StackAt(1).m_value))
	.yytext+"]"; }}
    /// <exclude/>
public class Item_1 : Item {
  public Item_1(Parser yyq):base(yyq){}}
  /// <exclude/>
public class Item_2 : Item {
  public Item_2(Parser yyq):base(yyq){}}
  /// <exclude/>
public class Item_2_1 : Item_2 {
  public Item_2_1(Parser yyq):base(yyq){ yytext=
	((ANY)(yyq.StackAt(0).m_value))
	.yytext; }}
    /// <exclude/>
public class Item_3 : Item {
  public Item_3(Parser yyq):base(yyq){}}
  /// <exclude/>
public class Item_4 : Item {
  public Item_4(Parser yyq):base(yyq){}}
  /// <exclude/>
public class Item_4_1 : Item_4 {
  public Item_4_1(Parser yyq):base(yyq){ yytext=
	((Name)(yyq.StackAt(0).m_value))
	.yytext; }}
    /// <exclude/>
public class Item_5 : Item {
  public Item_5(Parser yyq):base(yyq){}}
  /// <exclude/>
public class Item_6 : Item {
  public Item_6(Parser yyq):base(yyq){}}
  /// <exclude/>
public class Item_6_1 : Item_6 {
  public Item_6_1(Parser yyq):base(yyq){ yytext=";\n"; }}
  /// <exclude/>
public class Item_7 : Item {
  public Item_7(Parser yyq):base(yyq){}}
  /// <exclude/>
public class Item_8 : Item {
  public Item_8(Parser yyq):base(yyq){}}
  /// <exclude/>
public class Item_8_1 : Item_8 {
  public Item_8_1(Parser yyq):base(yyq){ yytext=" base "; }}
  /// <exclude/>
public class Item_9 : Item {
  public Item_9(Parser yyq):base(yyq){}}
  /// <exclude/>
public class Item_10 : Item {
  public Item_10(Parser yyq):base(yyq){}}
  /// <exclude/>
public class Item_10_1 : Item_10 {
  public Item_10_1(Parser yyq):base(yyq){ yytext=" this "; }}
  /// <exclude/>
public class Item_11 : Item {
  public Item_11(Parser yyq):base(yyq){}}
  /// <exclude/>
public class Item_12 : Item {
  public Item_12(Parser yyq):base(yyq){}}
  /// <exclude/>
public class Item_12_1 : Item_12 {
  public Item_12_1(Parser yyq):base(yyq){ yytext=" this["+
	((Stuff)(yyq.StackAt(1).m_value))
	.yytext+"]"; }}
    /// <exclude/>
public class Item_13 : Item {
  public Item_13(Parser yyq):base(yyq){}}
  /// <exclude/>
public class Item_14 : Item {
  public Item_14(Parser yyq):base(yyq){}}
  /// <exclude/>
public class Item_14_1 : Item_14 {
  public Item_14_1(Parser yyq):base(yyq){ yytext=":"; }}
  /// <exclude/>
public class Item_15 : Item {
  public Item_15(Parser yyq):base(yyq){}}
  /// <exclude/>
public class Item_16 : Item {
  public Item_16(Parser yyq):base(yyq){}}
  /// <exclude/>
public class Item_16_1 : Item_16 {
  public Item_16_1(Parser yyq):base(yyq){ yytext=" new "+
	((Call)(yyq.StackAt(0).m_value))
	.yytext; }}
    /// <exclude/>
public class Item_17 : Item {
  public Item_17(Parser yyq):base(yyq){}}
  /// <exclude/>
public class Item_18 : Item {
  public Item_18(Parser yyq):base(yyq){}}
  /// <exclude/>
public class Item_18_1 : Item_18 {
  public Item_18_1(Parser yyq):base(yyq){ yytext=" new "+
	((Name)(yyq.StackAt(0).m_value))
	.yytext; }}
    /// <exclude/>
public class Item_19 : Item {
  public Item_19(Parser yyq):base(yyq){}}
  /// <exclude/>
public class Item_20 : Item {
  public Item_20(Parser yyq):base(yyq){}}
  /// <exclude/>
public class Item_20_1 : Item_20 {
  public Item_20_1(Parser yyq):base(yyq){ yytext="("+
	((Stuff)(yyq.StackAt(1).m_value))
	.yytext+")"; }}
    /// <exclude/>
public class Item_21 : Item {
  public Item_21(Parser yyq):base(yyq){}}
  /// <exclude/>
public class Item_22 : Item {
  public Item_22(Parser yyq):base(yyq){}}
  /// <exclude/>
public class Item_22_1 : Item_22 {
  public Item_22_1(Parser yyq):base(yyq){ yytext="{"+
	((Stuff)(yyq.StackAt(1).m_value))
	.yytext+"}\n"; }}
    /// <exclude/>
public class Item_23 : Item {
  public Item_23(Parser yyq):base(yyq){}}
  /// <exclude/>
public class Item_24 : Item {
  public Item_24(Parser yyq):base(yyq){}}
  /// <exclude/>
public class Item_24_1 : Item_24 {
  public Item_24_1(Parser yyq):base(yyq){ yytext="[" + 
	((Stuff)(yyq.StackAt(1).m_value))
	.yytext + "]"; }}
    /// <exclude/>
public class yycs0syntax : YyParser {
  public override object Action(Parser yyq,SYMBOL yysym, int yyact) {
    switch(yyact) {
	 case -1: break; //// keep compiler happy
}  return null; }
/// <exclude/>
public yycs0syntax ():base() { arr = new int[] { 
101,4,6,52,0,
46,0,53,0,102,
20,103,4,18,67,
0,108,0,97,0,
115,0,115,0,66,
0,111,0,100,0,
121,0,1,19,1,
2,104,18,1,156,
102,2,0,105,5,
51,1,103,106,18,
1,103,107,20,108,
4,10,83,0,116,
0,117,0,102,0,
102,0,1,17,1,
2,2,0,1,102,
109,18,1,102,110,
20,111,4,12,76,
0,80,0,65,0,
82,0,69,0,78,
0,1,12,1,1,
2,0,1,101,112,
18,1,101,113,20,
114,4,8,78,0,
97,0,109,0,101,
0,1,23,1,2,
2,0,1,65,115,
18,1,65,116,20,
117,4,8,67,0,
97,0,108,0,108,
0,1,21,1,2,
2,0,1,63,118,
18,1,63,119,20,
120,4,12,82,0,
80,0,65,0,82,
0,69,0,78,0,
1,13,1,1,2,
0,1,155,121,18,
1,155,122,20,123,
4,12,82,0,66,
0,82,0,65,0,
67,0,69,0,1,
11,1,1,2,0,
1,56,124,18,1,
56,125,20,126,4,
8,73,0,116,0,
101,0,109,0,1,
18,1,2,2,0,
1,54,127,18,1,
54,128,20,129,4,
12,82,0,66,0,
82,0,65,0,67,
0,75,0,1,15,
1,1,2,0,1,
153,130,18,1,153,
125,2,0,1,157,
131,18,1,157,132,
23,133,4,6,69,
0,79,0,70,0,
1,2,1,6,2,
0,1,156,104,1,
149,134,18,1,149,
135,20,136,4,16,
66,0,97,0,115,
0,101,0,67,0,
97,0,108,0,108,
0,1,22,1,2,
2,0,1,154,137,
18,1,154,138,20,
139,4,8,67,0,
111,0,110,0,115,
0,1,20,1,2,
2,0,1,147,140,
18,1,147,119,2,
0,1,43,141,18,
1,43,107,2,0,
1,42,142,18,1,
42,143,20,144,4,
12,76,0,66,0,
82,0,65,0,67,
0,75,0,1,14,
1,1,2,0,1,
41,145,18,1,41,
146,20,147,4,4,
73,0,68,0,1,
6,1,1,2,0,
1,40,148,18,1,
40,149,20,150,4,
6,65,0,78,0,
89,0,1,7,1,
1,2,0,1,39,
151,18,1,39,113,
2,0,1,38,152,
18,1,38,153,20,
154,4,18,83,0,
69,0,77,0,73,
0,67,0,79,0,
76,0,79,0,78,
0,1,9,1,1,
2,0,1,37,155,
18,1,37,156,20,
157,4,8,66,0,
65,0,83,0,69,
0,1,3,1,1,
2,0,1,35,158,
18,1,35,128,2,
0,1,135,159,18,
1,135,107,2,0,
1,134,160,18,1,
134,110,2,0,1,
133,161,18,1,133,
156,2,0,1,29,
162,18,1,29,107,
2,0,1,28,163,
18,1,28,143,2,
0,1,27,164,18,
1,27,165,20,166,
4,8,84,0,72,
0,73,0,83,0,
1,4,1,1,2,
0,1,26,167,18,
1,26,168,20,169,
4,10,67,0,79,
0,76,0,79,0,
78,0,1,8,1,
1,2,0,1,131,
170,18,1,131,119,
2,0,1,115,171,
18,1,115,119,2,
0,1,21,172,18,
1,21,107,2,0,
1,20,173,18,1,
20,110,2,0,1,
19,174,18,1,19,
113,2,0,1,18,
175,18,1,18,176,
20,177,4,6,78,
0,69,0,87,0,
1,5,1,1,2,
0,1,119,178,18,
1,119,107,2,0,
1,16,179,18,1,
16,119,2,0,1,
117,180,18,1,117,
165,2,0,1,13,
181,18,1,13,107,
2,0,1,12,182,
18,1,12,110,2,
0,1,118,183,18,
1,118,110,2,0,
1,10,184,18,1,
10,122,2,0,1,
116,185,18,1,116,
168,2,0,1,8,
186,18,1,8,107,
2,0,1,7,187,
18,1,7,188,20,
189,4,12,76,0,
66,0,82,0,65,
0,67,0,69,0,
1,10,1,1,2,
0,1,5,190,18,
1,5,128,2,0,
1,4,191,18,1,
4,107,2,0,1,
3,192,18,1,3,
143,2,0,1,2,
193,18,1,2,194,
20,195,4,12,71,
0,83,0,116,0,
117,0,102,0,102,
0,1,16,1,2,
2,0,1,1,196,
18,1,1,188,2,
0,1,0,197,18,
1,0,0,2,0,
198,5,0,199,5,
97,1,98,200,19,
201,4,18,73,0,
116,0,101,0,109,
0,95,0,50,0,
52,0,95,0,49,
0,1,98,202,5,
10,1,21,203,16,
0,124,1,135,204,
16,0,124,1,13,
205,16,0,124,1,
103,206,16,0,124,
1,4,207,16,0,
124,1,8,208,16,
0,124,1,29,209,
16,0,124,1,119,
210,16,0,124,1,
2,211,16,0,130,
1,43,212,16,0,
124,1,97,213,19,
214,4,14,73,0,
116,0,101,0,109,
0,95,0,50,0,
52,0,1,97,202,
1,96,215,19,216,
4,14,73,0,116,
0,101,0,109,0,
95,0,50,0,51,
0,1,96,202,1,
95,217,19,218,4,
18,73,0,116,0,
101,0,109,0,95,
0,50,0,50,0,
95,0,49,0,1,
95,202,1,94,219,
19,220,4,14,73,
0,116,0,101,0,
109,0,95,0,50,
0,50,0,1,94,
202,1,93,221,19,
222,4,14,73,0,
116,0,101,0,109,
0,95,0,50,0,
49,0,1,93,202,
1,92,223,19,224,
4,18,73,0,116,
0,101,0,109,0,
95,0,50,0,48,
0,95,0,49,0,
1,92,202,1,91,
225,19,226,4,14,
73,0,116,0,101,
0,109,0,95,0,
50,0,48,0,1,
91,202,1,90,227,
19,228,4,14,73,
0,116,0,101,0,
109,0,95,0,49,
0,57,0,1,90,
202,1,89,229,19,
230,4,18,73,0,
116,0,101,0,109,
0,95,0,49,0,
56,0,95,0,49,
0,1,89,202,1,
88,231,19,232,4,
14,73,0,116,0,
101,0,109,0,95,
0,49,0,56,0,
1,88,202,1,87,
233,19,234,4,14,
73,0,116,0,101,
0,109,0,95,0,
49,0,55,0,1,
87,202,1,86,235,
19,236,4,18,73,
0,116,0,101,0,
109,0,95,0,49,
0,54,0,95,0,
49,0,1,86,202,
1,85,237,19,238,
4,14,73,0,116,
0,101,0,109,0,
95,0,49,0,54,
0,1,85,202,1,
84,239,19,240,4,
14,73,0,116,0,
101,0,109,0,95,
0,49,0,53,0,
1,84,202,1,83,
241,19,242,4,18,
73,0,116,0,101,
0,109,0,95,0,
49,0,52,0,95,
0,49,0,1,83,
202,1,82,243,19,
244,4,14,73,0,
116,0,101,0,109,
0,95,0,49,0,
52,0,1,82,202,
1,81,245,19,246,
4,14,73,0,116,
0,101,0,109,0,
95,0,49,0,51,
0,1,81,202,1,
80,247,19,248,4,
18,73,0,116,0,
101,0,109,0,95,
0,49,0,50,0,
95,0,49,0,1,
80,202,1,79,249,
19,250,4,14,73,
0,116,0,101,0,
109,0,95,0,49,
0,50,0,1,79,
202,1,78,251,19,
252,4,14,73,0,
116,0,101,0,109,
0,95,0,49,0,
49,0,1,78,202,
1,77,253,19,254,
4,18,73,0,116,
0,101,0,109,0,
95,0,49,0,48,
0,95,0,49,0,
1,77,202,1,76,
255,19,256,4,14,
73,0,116,0,101,
0,109,0,95,0,
49,0,48,0,1,
76,202,1,75,257,
19,258,4,12,73,
0,116,0,101,0,
109,0,95,0,57,
0,1,75,202,1,
74,259,19,260,4,
16,73,0,116,0,
101,0,109,0,95,
0,56,0,95,0,
49,0,1,74,202,
1,73,261,19,262,
4,12,73,0,116,
0,101,0,109,0,
95,0,56,0,1,
73,202,1,72,263,
19,264,4,12,73,
0,116,0,101,0,
109,0,95,0,55,
0,1,72,202,1,
71,265,19,266,4,
16,73,0,116,0,
101,0,109,0,95,
0,54,0,95,0,
49,0,1,71,202,
1,70,267,19,268,
4,12,73,0,116,
0,101,0,109,0,
95,0,54,0,1,
70,202,1,69,269,
19,270,4,12,73,
0,116,0,101,0,
109,0,95,0,53,
0,1,69,202,1,
68,271,19,272,4,
16,73,0,116,0,
101,0,109,0,95,
0,52,0,95,0,
49,0,1,68,202,
1,67,273,19,274,
4,12,73,0,116,
0,101,0,109,0,
95,0,52,0,1,
67,202,1,66,275,
19,276,4,12,73,
0,116,0,101,0,
109,0,95,0,51,
0,1,66,202,1,
65,277,19,278,4,
16,73,0,116,0,
101,0,109,0,95,
0,50,0,95,0,
49,0,1,65,202,
1,64,279,19,280,
4,12,73,0,116,
0,101,0,109,0,
95,0,50,0,1,
64,202,1,63,281,
19,282,4,12,73,
0,116,0,101,0,
109,0,95,0,49,
0,1,63,202,1,
62,283,19,284,4,
16,78,0,97,0,
109,0,101,0,95,
0,52,0,95,0,
49,0,1,62,285,
5,11,1,21,286,
16,0,151,1,135,
287,16,0,151,1,
18,288,16,0,174,
1,13,289,16,0,
151,1,103,290,16,
0,151,1,4,291,
16,0,151,1,8,
292,16,0,151,1,
29,293,16,0,151,
1,119,294,16,0,
151,1,2,295,16,
0,112,1,43,296,
16,0,151,1,61,
297,19,298,4,12,
78,0,97,0,109,
0,101,0,95,0,
52,0,1,61,285,
1,60,299,19,300,
4,12,78,0,97,
0,109,0,101,0,
95,0,51,0,1,
60,285,1,59,301,
19,302,4,16,78,
0,97,0,109,0,
101,0,95,0,50,
0,95,0,49,0,
1,59,285,1,58,
303,19,304,4,12,
78,0,97,0,109,
0,101,0,95,0,
50,0,1,58,285,
1,57,305,19,306,
4,12,78,0,97,
0,109,0,101,0,
95,0,49,0,1,
57,285,1,56,307,
19,308,4,24,66,
0,97,0,115,0,
101,0,67,0,97,
0,108,0,108,0,
95,0,54,0,95,
0,49,0,1,56,
309,5,1,1,115,
310,16,0,134,1,
55,311,19,312,4,
20,66,0,97,0,
115,0,101,0,67,
0,97,0,108,0,
108,0,95,0,54,
0,1,55,309,1,
54,313,19,314,4,
20,66,0,97,0,
115,0,101,0,67,
0,97,0,108,0,
108,0,95,0,53,
0,1,54,309,1,
53,315,19,316,4,
24,66,0,97,0,
115,0,101,0,67,
0,97,0,108,0,
108,0,95,0,52,
0,95,0,49,0,
1,53,309,1,52,
317,19,318,4,20,
66,0,97,0,115,
0,101,0,67,0,
97,0,108,0,108,
0,95,0,52,0,
1,52,309,1,51,
319,19,320,4,20,
66,0,97,0,115,
0,101,0,67,0,
97,0,108,0,108,
0,95,0,51,0,
1,51,309,1,50,
321,19,322,4,24,
66,0,97,0,115,
0,101,0,67,0,
97,0,108,0,108,
0,95,0,50,0,
95,0,49,0,1,
50,309,1,49,323,
19,324,4,20,66,
0,97,0,115,0,
101,0,67,0,97,
0,108,0,108,0,
95,0,50,0,1,
49,309,1,48,325,
19,326,4,20,66,
0,97,0,115,0,
101,0,67,0,97,
0,108,0,108,0,
95,0,49,0,1,
48,309,1,47,327,
19,328,4,16,67,
0,97,0,108,0,
108,0,95,0,50,
0,95,0,49,0,
1,47,329,5,1,
1,18,330,16,0,
115,1,46,331,19,
332,4,12,67,0,
97,0,108,0,108,
0,95,0,50,0,
1,46,329,1,45,
333,19,334,4,12,
67,0,97,0,108,
0,108,0,95,0,
49,0,1,45,329,
1,44,335,19,336,
4,16,67,0,111,
0,110,0,115,0,
95,0,50,0,95,
0,49,0,1,44,
337,5,1,1,2,
338,16,0,137,1,
43,339,19,340,4,
12,67,0,111,0,
110,0,115,0,95,
0,50,0,1,43,
337,1,42,341,19,
342,4,12,67,0,
111,0,110,0,115,
0,95,0,49,0,
1,42,337,1,41,
343,19,344,4,18,
83,0,116,0,117,
0,102,0,102,0,
95,0,52,0,95,
0,49,0,1,41,
345,5,9,1,42,
346,16,0,141,1,
20,347,16,0,172,
1,134,348,16,0,
159,1,12,349,16,
0,181,1,102,350,
16,0,106,1,3,
351,16,0,191,1,
7,352,16,0,186,
1,28,353,16,0,
162,1,118,354,16,
0,178,1,40,355,
19,356,4,14,83,
0,116,0,117,0,
102,0,102,0,95,
0,52,0,1,40,
345,1,39,357,19,
358,4,14,83,0,
116,0,117,0,102,
0,102,0,95,0,
51,0,1,39,345,
1,38,359,19,360,
4,18,83,0,116,
0,117,0,102,0,
102,0,95,0,50,
0,95,0,49,0,
1,38,345,1,37,
361,19,362,4,14,
83,0,116,0,117,
0,102,0,102,0,
95,0,50,0,1,
37,345,1,36,363,
19,364,4,14,83,
0,116,0,117,0,
102,0,102,0,95,
0,49,0,1,36,
345,1,35,365,19,
366,4,20,71,0,
83,0,116,0,117,
0,102,0,102,0,
95,0,54,0,95,
0,49,0,1,35,
367,5,1,1,1,
368,16,0,193,1,
34,369,19,370,4,
16,71,0,83,0,
116,0,117,0,102,
0,102,0,95,0,
54,0,1,34,367,
1,33,371,19,372,
4,16,71,0,83,
0,116,0,117,0,
102,0,102,0,95,
0,53,0,1,33,
367,1,32,373,19,
374,4,20,71,0,
83,0,116,0,117,
0,102,0,102,0,
95,0,52,0,95,
0,49,0,1,32,
367,1,31,375,19,
376,4,16,71,0,
83,0,116,0,117,
0,102,0,102,0,
95,0,52,0,1,
31,367,1,30,377,
19,378,4,16,71,
0,83,0,116,0,
117,0,102,0,102,
0,95,0,51,0,
1,30,367,1,29,
379,19,380,4,20,
71,0,83,0,116,
0,117,0,102,0,
102,0,95,0,50,
0,95,0,49,0,
1,29,367,1,28,
381,19,382,4,16,
71,0,83,0,116,
0,117,0,102,0,
102,0,95,0,50,
0,1,28,367,1,
27,383,19,384,4,
16,71,0,83,0,
116,0,117,0,102,
0,102,0,95,0,
49,0,1,27,367,
1,26,385,19,386,
4,26,67,0,108,
0,97,0,115,0,
115,0,66,0,111,
0,100,0,121,0,
95,0,50,0,95,
0,49,0,1,26,
387,5,1,1,0,
388,16,0,104,1,
25,389,19,390,4,
22,67,0,108,0,
97,0,115,0,115,
0,66,0,111,0,
100,0,121,0,95,
0,50,0,1,25,
387,1,24,391,19,
392,4,22,67,0,
108,0,97,0,115,
0,115,0,66,0,
111,0,100,0,121,
0,95,0,49,0,
1,24,387,1,23,
393,19,114,1,23,
285,1,22,394,19,
136,1,22,309,1,
21,395,19,117,1,
21,329,1,20,396,
19,139,1,20,337,
1,19,397,19,103,
1,19,387,1,18,
398,19,126,1,18,
202,1,17,399,19,
108,1,17,345,1,
16,400,19,195,1,
16,367,1,15,401,
19,129,1,15,402,
5,29,1,134,403,
17,404,15,405,4,
20,37,0,83,0,
116,0,117,0,102,
0,102,0,95,0,
50,0,95,0,49,
0,1,-1,1,5,
406,20,360,1,38,
1,3,1,1,1,
0,407,22,1,5,
1,43,408,16,0,
127,1,42,409,17,
404,1,0,407,1,
41,410,17,411,15,
412,4,18,37,0,
78,0,97,0,109,
0,101,0,95,0,
50,0,95,0,49,
0,1,-1,1,5,
413,20,302,1,59,
1,3,1,2,1,
1,414,22,1,12,
1,40,415,17,416,
15,417,4,18,37,
0,73,0,116,0,
101,0,109,0,95,
0,50,0,95,0,
49,0,1,-1,1,
5,418,20,278,1,
65,1,3,1,2,
1,1,419,22,1,
14,1,39,420,17,
421,15,422,4,18,
37,0,73,0,116,
0,101,0,109,0,
95,0,52,0,95,
0,49,0,1,-1,
1,5,423,20,272,
1,68,1,3,1,
2,1,1,424,22,
1,15,1,38,425,
17,426,15,427,4,
18,37,0,73,0,
116,0,101,0,109,
0,95,0,54,0,
95,0,49,0,1,
-1,1,5,428,20,
266,1,71,1,3,
1,2,1,1,429,
22,1,16,1,37,
430,17,431,15,432,
4,18,37,0,73,
0,116,0,101,0,
109,0,95,0,56,
0,95,0,49,0,
1,-1,1,5,433,
20,260,1,74,1,
3,1,2,1,1,
434,22,1,17,1,
35,435,17,436,15,
437,4,20,37,0,
73,0,116,0,101,
0,109,0,95,0,
49,0,50,0,95,
0,49,0,1,-1,
1,5,438,20,248,
1,80,1,3,1,
5,1,4,439,22,
1,19,1,29,440,
16,0,158,1,28,
441,17,404,1,0,
407,1,27,442,17,
443,15,444,4,20,
37,0,73,0,116,
0,101,0,109,0,
95,0,49,0,48,
0,95,0,49,0,
1,-1,1,5,445,
20,254,1,77,1,
3,1,2,1,1,
446,22,1,18,1,
26,447,17,448,15,
449,4,20,37,0,
73,0,116,0,101,
0,109,0,95,0,
49,0,52,0,95,
0,49,0,1,-1,
1,5,450,20,242,
1,83,1,3,1,
2,1,1,451,22,
1,20,1,118,452,
17,404,1,0,407,
1,16,453,17,454,
15,455,4,20,37,
0,73,0,116,0,
101,0,109,0,95,
0,50,0,48,0,
95,0,49,0,1,
-1,1,5,456,20,
224,1,92,1,3,
1,4,1,3,457,
22,1,23,1,20,
458,17,404,1,0,
407,1,19,459,17,
460,15,461,4,20,
37,0,73,0,116,
0,101,0,109,0,
95,0,49,0,56,
0,95,0,49,0,
1,-1,1,5,462,
20,230,1,89,1,
3,1,3,1,2,
463,22,1,22,1,
65,464,17,465,15,
466,4,20,37,0,
73,0,116,0,101,
0,109,0,95,0,
49,0,54,0,95,
0,49,0,1,-1,
1,5,467,20,236,
1,86,1,3,1,
3,1,2,468,22,
1,21,1,63,469,
17,470,15,471,4,
18,37,0,67,0,
97,0,108,0,108,
0,95,0,50,0,
95,0,49,0,1,
-1,1,5,472,20,
328,1,47,1,3,
1,5,1,4,473,
22,1,8,1,12,
474,17,404,1,0,
407,1,101,475,17,
421,1,1,424,1,
7,476,17,404,1,
0,407,1,10,477,
17,478,15,479,4,
20,37,0,73,0,
116,0,101,0,109,
0,95,0,50,0,
50,0,95,0,49,
0,1,-1,1,5,
480,20,218,1,95,
1,3,1,4,1,
3,481,22,1,24,
1,56,482,17,483,
15,484,4,20,37,
0,83,0,116,0,
117,0,102,0,102,
0,95,0,52,0,
95,0,49,0,1,
-1,1,5,485,20,
344,1,41,1,3,
1,3,1,2,486,
22,1,6,1,102,
487,17,404,1,0,
407,1,54,488,17,
489,15,490,4,18,
37,0,78,0,97,
0,109,0,101,0,
95,0,52,0,95,
0,49,0,1,-1,
1,5,491,20,284,
1,62,1,3,1,
5,1,4,492,22,
1,13,1,5,493,
17,494,15,495,4,
20,37,0,73,0,
116,0,101,0,109,
0,95,0,50,0,
52,0,95,0,49,
0,1,-1,1,5,
496,20,201,1,98,
1,3,1,4,1,
3,497,22,1,25,
1,4,498,16,0,
190,1,3,499,17,
404,1,0,407,1,
14,500,19,144,1,
14,501,5,43,1,
103,502,16,0,192,
1,102,487,1,101,
475,1,65,464,1,
63,469,1,56,482,
1,54,488,1,43,
503,16,0,192,1,
154,504,17,505,15,
506,4,22,37,0,
71,0,83,0,116,
0,117,0,102,0,
102,0,95,0,52,
0,95,0,49,0,
1,-1,1,5,507,
20,374,1,32,1,
3,1,3,1,2,
508,22,1,3,1,
153,509,17,510,15,
511,4,22,37,0,
71,0,83,0,116,
0,117,0,102,0,
102,0,95,0,54,
0,95,0,49,0,
1,-1,1,5,512,
20,366,1,35,1,
3,1,3,1,2,
513,22,1,4,1,
42,409,1,40,415,
1,149,514,17,515,
15,516,4,18,37,
0,67,0,111,0,
110,0,115,0,95,
0,50,0,95,0,
49,0,1,-1,1,
5,517,20,336,1,
44,1,3,1,6,
1,5,518,22,1,
7,1,41,519,16,
0,142,1,147,520,
17,521,15,522,4,
26,37,0,66,0,
97,0,115,0,101,
0,67,0,97,0,
108,0,108,0,95,
0,52,0,95,0,
49,0,1,-1,1,
5,523,20,316,1,
53,1,3,1,6,
1,5,524,22,1,
10,1,39,420,1,
38,425,1,37,430,
1,35,435,1,134,
403,1,28,441,1,
29,525,16,0,192,
1,135,526,16,0,
192,1,27,527,16,
0,163,1,26,447,
1,131,528,17,529,
15,530,4,26,37,
0,66,0,97,0,
115,0,101,0,67,
0,97,0,108,0,
108,0,95,0,54,
0,95,0,49,0,
1,-1,1,5,531,
20,308,1,56,1,
3,1,6,1,5,
532,22,1,11,1,
21,533,16,0,192,
1,20,458,1,19,
459,1,8,534,16,
0,192,1,16,453,
1,12,474,1,13,
535,16,0,192,1,
119,536,16,0,192,
1,118,452,1,10,
477,1,115,537,17,
538,15,539,4,26,
37,0,66,0,97,
0,115,0,101,0,
67,0,97,0,108,
0,108,0,95,0,
50,0,95,0,49,
0,1,-1,1,5,
540,20,322,1,50,
1,3,1,1,1,
0,541,22,1,9,
1,7,476,1,5,
493,1,4,542,16,
0,192,1,3,499,
1,2,543,16,0,
192,1,1,544,17,
545,15,546,4,22,
37,0,71,0,83,
0,116,0,117,0,
102,0,102,0,95,
0,50,0,95,0,
49,0,1,-1,1,
5,547,20,380,1,
29,1,3,1,1,
1,0,548,22,1,
2,1,13,549,19,
120,1,13,550,5,
31,1,41,410,1,
40,415,1,42,409,
1,135,551,16,0,
140,1,134,403,1,
39,420,1,38,425,
1,37,430,1,35,
435,1,118,452,1,
28,441,1,27,442,
1,26,447,1,119,
552,16,0,170,1,
12,474,1,16,453,
1,21,553,16,0,
118,1,20,458,1,
19,459,1,65,464,
1,63,469,1,103,
554,16,0,171,1,
13,555,16,0,179,
1,101,475,1,7,
476,1,10,477,1,
56,482,1,102,487,
1,54,488,1,5,
493,1,3,499,1,
12,556,19,111,1,
12,557,5,45,1,
103,558,16,0,182,
1,102,487,1,101,
559,16,0,109,1,
65,464,1,63,469,
1,56,482,1,54,
488,1,43,560,16,
0,182,1,154,504,
1,153,509,1,42,
409,1,40,415,1,
149,514,1,41,410,
1,147,520,1,39,
420,1,38,425,1,
37,430,1,35,435,
1,134,403,1,133,
561,16,0,160,1,
28,441,1,29,562,
16,0,182,1,135,
563,16,0,182,1,
27,442,1,26,447,
1,131,528,1,8,
564,16,0,182,1,
21,565,16,0,182,
1,20,458,1,19,
566,16,0,173,1,
10,477,1,16,453,
1,12,474,1,13,
567,16,0,182,1,
119,568,16,0,182,
1,118,452,1,117,
569,16,0,183,1,
115,537,1,7,476,
1,5,493,1,4,
570,16,0,182,1,
3,499,1,2,571,
16,0,182,1,1,
544,1,11,572,19,
123,1,11,573,5,
35,1,102,487,1,
101,475,1,65,464,
1,63,469,1,56,
482,1,54,488,1,
154,504,1,153,509,
1,42,409,1,40,
415,1,149,514,1,
41,410,1,147,520,
1,39,420,1,38,
425,1,37,430,1,
35,435,1,134,403,
1,28,441,1,27,
442,1,26,447,1,
131,528,1,20,458,
1,19,459,1,16,
453,1,8,574,16,
0,184,1,12,474,
1,118,452,1,10,
477,1,115,537,1,
7,476,1,5,493,
1,3,499,1,2,
575,16,0,121,1,
1,544,1,10,576,
19,189,1,10,577,
5,44,1,103,578,
16,0,187,1,102,
487,1,101,475,1,
65,464,1,63,469,
1,56,482,1,54,
488,1,43,579,16,
0,187,1,154,504,
1,153,509,1,42,
409,1,40,415,1,
149,514,1,41,410,
1,147,520,1,39,
420,1,38,425,1,
37,430,1,35,435,
1,134,403,1,28,
441,1,29,580,16,
0,187,1,135,581,
16,0,187,1,27,
442,1,26,447,1,
131,528,1,21,582,
16,0,187,1,20,
458,1,19,459,1,
8,583,16,0,187,
1,16,453,1,12,
474,1,13,584,16,
0,187,1,119,585,
16,0,187,1,118,
452,1,10,477,1,
115,537,1,7,476,
1,5,493,1,4,
586,16,0,187,1,
3,499,1,2,587,
16,0,187,1,1,
544,1,0,588,16,
0,196,1,9,589,
19,154,1,9,590,
5,43,1,103,591,
16,0,152,1,102,
487,1,101,475,1,
65,464,1,63,469,
1,56,482,1,54,
488,1,43,592,16,
0,152,1,154,504,
1,153,509,1,42,
409,1,40,415,1,
149,514,1,41,410,
1,147,520,1,39,
420,1,38,425,1,
37,430,1,35,435,
1,134,403,1,28,
441,1,29,593,16,
0,152,1,135,594,
16,0,152,1,27,
442,1,26,447,1,
131,528,1,21,595,
16,0,152,1,20,
458,1,19,459,1,
8,596,16,0,152,
1,16,453,1,12,
474,1,13,597,16,
0,152,1,119,598,
16,0,152,1,118,
452,1,10,477,1,
115,537,1,7,476,
1,5,493,1,4,
599,16,0,152,1,
3,499,1,2,600,
16,0,152,1,1,
544,1,8,601,19,
169,1,8,602,5,
43,1,103,603,16,
0,167,1,102,487,
1,101,475,1,65,
464,1,63,469,1,
56,482,1,54,488,
1,43,604,16,0,
167,1,154,504,1,
153,509,1,42,409,
1,40,415,1,149,
514,1,41,410,1,
147,520,1,39,420,
1,38,425,1,37,
430,1,35,435,1,
134,403,1,28,441,
1,29,605,16,0,
167,1,135,606,16,
0,167,1,27,442,
1,26,447,1,131,
528,1,21,607,16,
0,167,1,20,458,
1,19,459,1,8,
608,16,0,167,1,
16,453,1,12,474,
1,13,609,16,0,
167,1,119,610,16,
0,167,1,118,452,
1,10,477,1,115,
611,16,0,185,1,
7,476,1,5,493,
1,4,612,16,0,
167,1,3,499,1,
2,613,16,0,167,
1,1,544,1,7,
614,19,150,1,7,
615,5,43,1,103,
616,16,0,148,1,
102,487,1,101,475,
1,65,464,1,63,
469,1,56,482,1,
54,488,1,43,617,
16,0,148,1,154,
504,1,153,509,1,
42,409,1,40,415,
1,149,514,1,41,
410,1,147,520,1,
39,420,1,38,425,
1,37,430,1,35,
435,1,134,403,1,
28,441,1,29,618,
16,0,148,1,135,
619,16,0,148,1,
27,442,1,26,447,
1,131,528,1,21,
620,16,0,148,1,
20,458,1,19,459,
1,8,621,16,0,
148,1,16,453,1,
12,474,1,13,622,
16,0,148,1,119,
623,16,0,148,1,
118,452,1,10,477,
1,115,537,1,7,
476,1,5,493,1,
4,624,16,0,148,
1,3,499,1,2,
625,16,0,148,1,
1,544,1,6,626,
19,147,1,6,627,
5,44,1,103,628,
16,0,145,1,102,
487,1,101,475,1,
65,464,1,63,469,
1,56,482,1,54,
488,1,43,629,16,
0,145,1,154,504,
1,153,509,1,42,
409,1,40,415,1,
149,514,1,41,410,
1,147,520,1,39,
420,1,38,425,1,
37,430,1,35,435,
1,134,403,1,28,
441,1,29,630,16,
0,145,1,135,631,
16,0,145,1,27,
442,1,26,447,1,
131,528,1,8,632,
16,0,145,1,21,
633,16,0,145,1,
20,458,1,19,459,
1,18,634,16,0,
145,1,16,453,1,
12,474,1,13,635,
16,0,145,1,119,
636,16,0,145,1,
118,452,1,10,477,
1,115,537,1,7,
476,1,5,493,1,
4,637,16,0,145,
1,3,499,1,2,
638,16,0,145,1,
1,544,1,5,639,
19,177,1,5,640,
5,43,1,103,641,
16,0,175,1,102,
487,1,101,475,1,
65,464,1,63,469,
1,56,482,1,54,
488,1,43,642,16,
0,175,1,154,504,
1,153,509,1,42,
409,1,40,415,1,
149,514,1,41,410,
1,147,520,1,39,
420,1,38,425,1,
37,430,1,35,435,
1,134,403,1,28,
441,1,29,643,16,
0,175,1,135,644,
16,0,175,1,27,
442,1,26,447,1,
131,528,1,21,645,
16,0,175,1,20,
458,1,19,459,1,
8,646,16,0,175,
1,16,453,1,12,
474,1,13,647,16,
0,175,1,119,648,
16,0,175,1,118,
452,1,10,477,1,
115,537,1,7,476,
1,5,493,1,4,
649,16,0,175,1,
3,499,1,2,650,
16,0,175,1,1,
544,1,4,651,19,
166,1,4,652,5,
44,1,103,653,16,
0,164,1,102,487,
1,101,475,1,65,
464,1,63,469,1,
56,482,1,54,488,
1,43,654,16,0,
164,1,154,504,1,
153,509,1,42,409,
1,40,415,1,149,
514,1,41,410,1,
147,520,1,39,420,
1,38,425,1,37,
430,1,35,435,1,
134,403,1,28,441,
1,29,655,16,0,
164,1,135,656,16,
0,164,1,27,442,
1,26,447,1,131,
528,1,21,657,16,
0,164,1,20,458,
1,19,459,1,8,
658,16,0,164,1,
16,453,1,12,474,
1,13,659,16,0,
164,1,119,660,16,
0,164,1,118,452,
1,10,477,1,116,
661,16,0,180,1,
115,537,1,7,476,
1,5,493,1,4,
662,16,0,164,1,
3,499,1,2,663,
16,0,164,1,1,
544,1,3,664,19,
157,1,3,665,5,
44,1,103,666,16,
0,155,1,102,487,
1,101,475,1,65,
464,1,63,469,1,
56,482,1,54,488,
1,43,667,16,0,
155,1,154,504,1,
153,509,1,42,409,
1,40,415,1,149,
514,1,41,410,1,
147,520,1,39,420,
1,38,425,1,37,
430,1,35,435,1,
134,403,1,28,441,
1,29,668,16,0,
155,1,135,669,16,
0,155,1,27,442,
1,26,447,1,131,
528,1,21,670,16,
0,155,1,20,458,
1,19,459,1,8,
671,16,0,155,1,
16,453,1,12,474,
1,13,672,16,0,
155,1,119,673,16,
0,155,1,118,452,
1,10,477,1,116,
674,16,0,161,1,
115,537,1,7,476,
1,5,493,1,4,
675,16,0,155,1,
3,499,1,2,676,
16,0,155,1,1,
544,1,2,677,19,
133,1,2,678,5,
1,1,155,679,17,
680,15,681,4,28,
37,0,67,0,108,
0,97,0,115,0,
115,0,66,0,111,
0,100,0,121,0,
95,0,50,0,95,
0,49,0,1,-1,
1,5,682,20,386,
1,26,1,3,1,
4,1,3,683,22,
1,1,2,1,0};
new Sfactory(this,"Item_20_1",new SCreator(Item_20_1_factory));
new Sfactory(this,"Item_23",new SCreator(Item_23_factory));
new Sfactory(this,"Item_6",new SCreator(Item_6_factory));
new Sfactory(this,"Stuff_4_1",new SCreator(Stuff_4_1_factory));
new Sfactory(this,"Item_15",new SCreator(Item_15_factory));
new Sfactory(this,"Item_22_1",new SCreator(Item_22_1_factory));
new Sfactory(this,"Item_2_1",new SCreator(Item_2_1_factory));
new Sfactory(this,"Item_11",new SCreator(Item_11_factory));
new Sfactory(this,"Item_8",new SCreator(Item_8_factory));
new Sfactory(this,"Name",new SCreator(Name_factory));
new Sfactory(this,"BaseCall_4",new SCreator(BaseCall_4_factory));
new Sfactory(this,"Item_14",new SCreator(Item_14_factory));
new Sfactory(this,"Cons_2",new SCreator(Cons_2_factory));
new Sfactory(this,"BaseCall_3",new SCreator(BaseCall_3_factory));
new Sfactory(this,"Item_17",new SCreator(Item_17_factory));
new Sfactory(this,"Item_10",new SCreator(Item_10_factory));
new Sfactory(this,"ClassBody_2_1",new SCreator(ClassBody_2_1_factory));
new Sfactory(this,"Call_2",new SCreator(Call_2_factory));
new Sfactory(this,"Item_14_1",new SCreator(Item_14_1_factory));
new Sfactory(this,"Item_1",new SCreator(Item_1_factory));
new Sfactory(this,"GStuff_6_1",new SCreator(GStuff_6_1_factory));
new Sfactory(this,"Item_6_1",new SCreator(Item_6_1_factory));
new Sfactory(this,"Call_2_1",new SCreator(Call_2_1_factory));
new Sfactory(this,"Item_5",new SCreator(Item_5_factory));
new Sfactory(this,"Item_4_1",new SCreator(Item_4_1_factory));
new Sfactory(this,"BaseCall",new SCreator(BaseCall_factory));
new Sfactory(this,"Item_10_1",new SCreator(Item_10_1_factory));
new Sfactory(this,"Name_2",new SCreator(Name_2_factory));
new Sfactory(this,"Call_1",new SCreator(Call_1_factory));
new Sfactory(this,"ClassBody",new SCreator(ClassBody_factory));
new Sfactory(this,"error",new SCreator(error_factory));
new Sfactory(this,"Item_8_1",new SCreator(Item_8_1_factory));
new Sfactory(this,"BaseCall_6_1",new SCreator(BaseCall_6_1_factory));
new Sfactory(this,"Item_16_1",new SCreator(Item_16_1_factory));
new Sfactory(this,"Item_13",new SCreator(Item_13_factory));
new Sfactory(this,"BaseCall_2",new SCreator(BaseCall_2_factory));
new Sfactory(this,"Name_2_1",new SCreator(Name_2_1_factory));
new Sfactory(this,"Cons_2_1",new SCreator(Cons_2_1_factory));
new Sfactory(this,"Item_7",new SCreator(Item_7_factory));
new Sfactory(this,"BaseCall_2_1",new SCreator(BaseCall_2_1_factory));
new Sfactory(this,"GStuff_4",new SCreator(GStuff_4_factory));
new Sfactory(this,"Item_22",new SCreator(Item_22_factory));
new Sfactory(this,"Name_3",new SCreator(Name_3_factory));
new Sfactory(this,"GStuff_3",new SCreator(GStuff_3_factory));
new Sfactory(this,"Stuff_2_1",new SCreator(Stuff_2_1_factory));
new Sfactory(this,"ClassBody_1",new SCreator(ClassBody_1_factory));
new Sfactory(this,"Name_4",new SCreator(Name_4_factory));
new Sfactory(this,"BaseCall_6",new SCreator(BaseCall_6_factory));
new Sfactory(this,"Item_24_1",new SCreator(Item_24_1_factory));
new Sfactory(this,"Item_12_1",new SCreator(Item_12_1_factory));
new Sfactory(this,"GStuff_1",new SCreator(GStuff_1_factory));
new Sfactory(this,"Item_4",new SCreator(Item_4_factory));
new Sfactory(this,"GStuff_2_1",new SCreator(GStuff_2_1_factory));
new Sfactory(this,"Item_20",new SCreator(Item_20_factory));
new Sfactory(this,"Stuff_1",new SCreator(Stuff_1_factory));
new Sfactory(this,"Stuff",new SCreator(Stuff_factory));
new Sfactory(this,"Item_3",new SCreator(Item_3_factory));
new Sfactory(this,"Name_4_1",new SCreator(Name_4_1_factory));
new Sfactory(this,"Item_16",new SCreator(Item_16_factory));
new Sfactory(this,"Stuff_4",new SCreator(Stuff_4_factory));
new Sfactory(this,"Cons",new SCreator(Cons_factory));
new Sfactory(this,"Item_18_1",new SCreator(Item_18_1_factory));
new Sfactory(this,"GStuff_5",new SCreator(GStuff_5_factory));
new Sfactory(this,"Item_21",new SCreator(Item_21_factory));
new Sfactory(this,"Stuff_2",new SCreator(Stuff_2_factory));
new Sfactory(this,"Item_18",new SCreator(Item_18_factory));
new Sfactory(this,"GStuff_4_1",new SCreator(GStuff_4_1_factory));
new Sfactory(this,"BaseCall_1",new SCreator(BaseCall_1_factory));
new Sfactory(this,"Item_24",new SCreator(Item_24_factory));
new Sfactory(this,"Item",new SCreator(Item_factory));
new Sfactory(this,"ClassBody_2",new SCreator(ClassBody_2_factory));
new Sfactory(this,"GStuff",new SCreator(GStuff_factory));
new Sfactory(this,"Item_2",new SCreator(Item_2_factory));
new Sfactory(this,"Item_12",new SCreator(Item_12_factory));
new Sfactory(this,"Item_9",new SCreator(Item_9_factory));
new Sfactory(this,"Stuff_3",new SCreator(Stuff_3_factory));
new Sfactory(this,"Name_1",new SCreator(Name_1_factory));
new Sfactory(this,"Call",new SCreator(Call_factory));
new Sfactory(this,"BaseCall_5",new SCreator(BaseCall_5_factory));
new Sfactory(this,"BaseCall_4_1",new SCreator(BaseCall_4_1_factory));
new Sfactory(this,"Cons_1",new SCreator(Cons_1_factory));
new Sfactory(this,"Item_19",new SCreator(Item_19_factory));
new Sfactory(this,"GStuff_6",new SCreator(GStuff_6_factory));
new Sfactory(this,"GStuff_2",new SCreator(GStuff_2_factory));
}
public static object Item_20_1_factory(Parser yyp) { return new Item_20_1(yyp); }
public static object Item_23_factory(Parser yyp) { return new Item_23(yyp); }
public static object Item_6_factory(Parser yyp) { return new Item_6(yyp); }
public static object Stuff_4_1_factory(Parser yyp) { return new Stuff_4_1(yyp); }
public static object Item_15_factory(Parser yyp) { return new Item_15(yyp); }
public static object Item_22_1_factory(Parser yyp) { return new Item_22_1(yyp); }
public static object Item_2_1_factory(Parser yyp) { return new Item_2_1(yyp); }
public static object Item_11_factory(Parser yyp) { return new Item_11(yyp); }
public static object Item_8_factory(Parser yyp) { return new Item_8(yyp); }
public static object Name_factory(Parser yyp) { return new Name(yyp); }
public static object BaseCall_4_factory(Parser yyp) { return new BaseCall_4(yyp); }
public static object Item_14_factory(Parser yyp) { return new Item_14(yyp); }
public static object Cons_2_factory(Parser yyp) { return new Cons_2(yyp); }
public static object BaseCall_3_factory(Parser yyp) { return new BaseCall_3(yyp); }
public static object Item_17_factory(Parser yyp) { return new Item_17(yyp); }
public static object Item_10_factory(Parser yyp) { return new Item_10(yyp); }
public static object ClassBody_2_1_factory(Parser yyp) { return new ClassBody_2_1(yyp); }
public static object Call_2_factory(Parser yyp) { return new Call_2(yyp); }
public static object Item_14_1_factory(Parser yyp) { return new Item_14_1(yyp); }
public static object Item_1_factory(Parser yyp) { return new Item_1(yyp); }
public static object GStuff_6_1_factory(Parser yyp) { return new GStuff_6_1(yyp); }
public static object Item_6_1_factory(Parser yyp) { return new Item_6_1(yyp); }
public static object Call_2_1_factory(Parser yyp) { return new Call_2_1(yyp); }
public static object Item_5_factory(Parser yyp) { return new Item_5(yyp); }
public static object Item_4_1_factory(Parser yyp) { return new Item_4_1(yyp); }
public static object BaseCall_factory(Parser yyp) { return new BaseCall(yyp); }
public static object Item_10_1_factory(Parser yyp) { return new Item_10_1(yyp); }
public static object Name_2_factory(Parser yyp) { return new Name_2(yyp); }
public static object Call_1_factory(Parser yyp) { return new Call_1(yyp); }
public static object ClassBody_factory(Parser yyp) { return new ClassBody(yyp); }
public static object error_factory(Parser yyp) { return new error(yyp); }
public static object Item_8_1_factory(Parser yyp) { return new Item_8_1(yyp); }
public static object BaseCall_6_1_factory(Parser yyp) { return new BaseCall_6_1(yyp); }
public static object Item_16_1_factory(Parser yyp) { return new Item_16_1(yyp); }
public static object Item_13_factory(Parser yyp) { return new Item_13(yyp); }
public static object BaseCall_2_factory(Parser yyp) { return new BaseCall_2(yyp); }
public static object Name_2_1_factory(Parser yyp) { return new Name_2_1(yyp); }
public static object Cons_2_1_factory(Parser yyp) { return new Cons_2_1(yyp); }
public static object Item_7_factory(Parser yyp) { return new Item_7(yyp); }
public static object BaseCall_2_1_factory(Parser yyp) { return new BaseCall_2_1(yyp); }
public static object GStuff_4_factory(Parser yyp) { return new GStuff_4(yyp); }
public static object Item_22_factory(Parser yyp) { return new Item_22(yyp); }
public static object Name_3_factory(Parser yyp) { return new Name_3(yyp); }
public static object GStuff_3_factory(Parser yyp) { return new GStuff_3(yyp); }
public static object Stuff_2_1_factory(Parser yyp) { return new Stuff_2_1(yyp); }
public static object ClassBody_1_factory(Parser yyp) { return new ClassBody_1(yyp); }
public static object Name_4_factory(Parser yyp) { return new Name_4(yyp); }
public static object BaseCall_6_factory(Parser yyp) { return new BaseCall_6(yyp); }
public static object Item_24_1_factory(Parser yyp) { return new Item_24_1(yyp); }
public static object Item_12_1_factory(Parser yyp) { return new Item_12_1(yyp); }
public static object GStuff_1_factory(Parser yyp) { return new GStuff_1(yyp); }
public static object Item_4_factory(Parser yyp) { return new Item_4(yyp); }
public static object GStuff_2_1_factory(Parser yyp) { return new GStuff_2_1(yyp); }
public static object Item_20_factory(Parser yyp) { return new Item_20(yyp); }
public static object Stuff_1_factory(Parser yyp) { return new Stuff_1(yyp); }
public static object Stuff_factory(Parser yyp) { return new Stuff(yyp); }
public static object Item_3_factory(Parser yyp) { return new Item_3(yyp); }
public static object Name_4_1_factory(Parser yyp) { return new Name_4_1(yyp); }
public static object Item_16_factory(Parser yyp) { return new Item_16(yyp); }
public static object Stuff_4_factory(Parser yyp) { return new Stuff_4(yyp); }
public static object Cons_factory(Parser yyp) { return new Cons(yyp); }
public static object Item_18_1_factory(Parser yyp) { return new Item_18_1(yyp); }
public static object GStuff_5_factory(Parser yyp) { return new GStuff_5(yyp); }
public static object Item_21_factory(Parser yyp) { return new Item_21(yyp); }
public static object Stuff_2_factory(Parser yyp) { return new Stuff_2(yyp); }
public static object Item_18_factory(Parser yyp) { return new Item_18(yyp); }
public static object GStuff_4_1_factory(Parser yyp) { return new GStuff_4_1(yyp); }
public static object BaseCall_1_factory(Parser yyp) { return new BaseCall_1(yyp); }
public static object Item_24_factory(Parser yyp) { return new Item_24(yyp); }
public static object Item_factory(Parser yyp) { return new Item(yyp); }
public static object ClassBody_2_factory(Parser yyp) { return new ClassBody_2(yyp); }
public static object GStuff_factory(Parser yyp) { return new GStuff(yyp); }
public static object Item_2_factory(Parser yyp) { return new Item_2(yyp); }
public static object Item_12_factory(Parser yyp) { return new Item_12(yyp); }
public static object Item_9_factory(Parser yyp) { return new Item_9(yyp); }
public static object Stuff_3_factory(Parser yyp) { return new Stuff_3(yyp); }
public static object Name_1_factory(Parser yyp) { return new Name_1(yyp); }
public static object Call_factory(Parser yyp) { return new Call(yyp); }
public static object BaseCall_5_factory(Parser yyp) { return new BaseCall_5(yyp); }
public static object BaseCall_4_1_factory(Parser yyp) { return new BaseCall_4_1(yyp); }
public static object Cons_1_factory(Parser yyp) { return new Cons_1(yyp); }
public static object Item_19_factory(Parser yyp) { return new Item_19(yyp); }
public static object GStuff_6_factory(Parser yyp) { return new GStuff_6(yyp); }
public static object GStuff_2_factory(Parser yyp) { return new GStuff_2(yyp); }
}
/// <exclude/>
public class cs0syntax : Parser {
public cs0syntax ():base(new yycs0syntax (),new cs0tokens()) {}
public cs0syntax (YyParser syms):base(syms,new cs0tokens()) {}
public cs0syntax (YyParser syms,ErrorHandler erh):base(syms,new cs0tokens(erh)) {}

	public string Out;
	public string Cls;
	public string Par;
	public string Ctx;
	public bool defconseen = false;

 }
}
