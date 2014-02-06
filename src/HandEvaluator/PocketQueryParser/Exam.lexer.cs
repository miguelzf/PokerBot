using System;using Tools;
#pragma warning disable 0162
#pragma warning disable 1591
//%Card+3
/// <exclude/>
public class Card : TOKEN{ public override string yyname { get { return "Card";}}
/// <exclude/>
public override int yynum { get { return 3; }}
/// <exclude/>
 public Card(Lexer yyl):base(yyl) {}}
/// <exclude/>
public class Card_1 : Card {
  /// <exclude/>
  public Card_1(Lexer yym):base(yym) 		{yylval = yytext.Clone();}}
//%Card169+5
/// <exclude/>
public class Card169 : TOKEN{ public override string yyname { get { return "Card169";}}
/// <exclude/>
public override int yynum { get { return 5; }}
/// <exclude/>
 public Card169(Lexer yyl):base(yyl) {}}
/// <exclude/>
public class Card169_1 : Card169 {
  /// <exclude/>
  public Card169_1(Lexer yym):base(yym) 	{yylval = yytext.Clone();}}
//%Card169Wild+7
/// <exclude/>
public class Card169Wild : TOKEN{ public override string yyname { get { return "Card169Wild";}}
/// <exclude/>
public override int yynum { get { return 7; }}
/// <exclude/>
 public Card169Wild(Lexer yyl):base(yyl) {}}
/// <exclude/>
public class Card169Wild_1 : Card169Wild {
  /// <exclude/>
  public Card169Wild_1(Lexer yym):base(yym)  {yylval = yytext.Clone();}}
//%Percent+9
/// <exclude/>
public class Percent : TOKEN{ public override string yyname { get { return "Percent";}}
/// <exclude/>
public override int yynum { get { return 9; }}
/// <exclude/>
 public Percent(Lexer yyl):base(yyl) {}}
/// <exclude/>
public class Percent_1 : Percent {
  /// <exclude/>
  public Percent_1(Lexer yym):base(yym) 	{yylval = yytext.Substring(0, yytext.Length-1);}}
//%Number+11
/// <exclude/>
public class Number : TOKEN{ public override string yyname { get { return "Number";}}
/// <exclude/>
public override int yynum { get { return 11; }}
/// <exclude/>
 public Number(Lexer yyl):base(yyl) {}}
/// <exclude/>
public class Number_1 : Number {
  /// <exclude/>
  public Number_1(Lexer yym):base(yym) 		{yylval = yytext.Clone();}}
//%Any+13
/// <exclude/>
public class Any : TOKEN{ public override string yyname { get { return "Any";}}
/// <exclude/>
public override int yynum { get { return 13; }}
/// <exclude/>
 public Any(Lexer yyl):base(yyl) {}}
/// <exclude/>
public class Any_1 : Any {
  /// <exclude/>
  public Any_1(Lexer yym):base(yym) 		{yylval = yytext.Clone();}}
//%Gapped+15
/// <exclude/>
public class Gapped : TOKEN{ public override string yyname { get { return "Gapped";}}
/// <exclude/>
public override int yynum { get { return 15; }}
/// <exclude/>
 public Gapped(Lexer yyl):base(yyl) {}}
/// <exclude/>
public class Gapped_1 : Gapped {
  /// <exclude/>
  public Gapped_1(Lexer yym):base(yym) 		{yylval = yytext.Clone();}}
//%Gapped1+17
/// <exclude/>
public class Gapped1 : TOKEN{ public override string yyname { get { return "Gapped1";}}
/// <exclude/>
public override int yynum { get { return 17; }}
/// <exclude/>
 public Gapped1(Lexer yyl):base(yyl) {}}
/// <exclude/>
public class Gapped1_1 : Gapped1 {
  /// <exclude/>
  public Gapped1_1(Lexer yym):base(yym) 	{yylval = yytext.Clone();}}
//%Gapped2+19
/// <exclude/>
public class Gapped2 : TOKEN{ public override string yyname { get { return "Gapped2";}}
/// <exclude/>
public override int yynum { get { return 19; }}
/// <exclude/>
 public Gapped2(Lexer yyl):base(yyl) {}}
/// <exclude/>
public class Gapped2_1 : Gapped2 {
  /// <exclude/>
  public Gapped2_1(Lexer yym):base(yym) 	{yylval = yytext.Clone();}}
//%Gapped3+21
/// <exclude/>
public class Gapped3 : TOKEN{ public override string yyname { get { return "Gapped3";}}
/// <exclude/>
public override int yynum { get { return 21; }}
/// <exclude/>
 public Gapped3(Lexer yyl):base(yyl) {}}
/// <exclude/>
public class Gapped3_1 : Gapped3 {
  /// <exclude/>
  public Gapped3_1(Lexer yym):base(yym) 	{yylval = yytext.Clone();}}
//%To+23
/// <exclude/>
public class To : TOKEN{ public override string yyname { get { return "To";}}
/// <exclude/>
public override int yynum { get { return 23; }}
/// <exclude/>
 public To(Lexer yyl):base(yyl) {}}
/// <exclude/>
public class To_1 : To {
  /// <exclude/>
  public To_1(Lexer yym):base(yym) 			{yylval = yytext.Clone();}}
/// <exclude/>
public class To_2 : To {
  /// <exclude/>
  public To_2(Lexer yym):base(yym) 			{yylval = yytext.Clone();}}
/// <exclude/>
public class To_3 : To {
  /// <exclude/>
  public To_3(Lexer yym):base(yym) 			{yylval = yytext.Clone();}}
//%Pair+27
/// <exclude/>
public class Pair : TOKEN{ public override string yyname { get { return "Pair";}}
/// <exclude/>
public override int yynum { get { return 27; }}
/// <exclude/>
 public Pair(Lexer yyl):base(yyl) {}}
/// <exclude/>
public class Pair_1 : Pair {
  /// <exclude/>
  public Pair_1(Lexer yym):base(yym) 		{yylval = yytext.Clone();}}
//%Not+29
/// <exclude/>
public class Not : TOKEN{ public override string yyname { get { return "Not";}}
/// <exclude/>
public override int yynum { get { return 29; }}
/// <exclude/>
 public Not(Lexer yyl):base(yyl) {}}
/// <exclude/>
public class Not_1 : Not {
  /// <exclude/>
  public Not_1(Lexer yym):base(yym) 		{yylval = yytext.Clone();}}
/// <exclude/>
public class Not_2 : Not {
  /// <exclude/>
  public Not_2(Lexer yym):base(yym) 		{yylval = yytext.Clone();}}
//%But+32
/// <exclude/>
public class But : TOKEN{ public override string yyname { get { return "But";}}
/// <exclude/>
public override int yynum { get { return 32; }}
/// <exclude/>
 public But(Lexer yyl):base(yyl) {}}
/// <exclude/>
public class But_1 : But {
  /// <exclude/>
  public But_1(Lexer yym):base(yym) 		{yylval = yytext.Clone();}}
//%Connected+34
/// <exclude/>
public class Connected : TOKEN{ public override string yyname { get { return "Connected";}}
/// <exclude/>
public override int yynum { get { return 34; }}
/// <exclude/>
 public Connected(Lexer yyl):base(yyl) {}}
/// <exclude/>
public class Connected_1 : Connected {
  /// <exclude/>
  public Connected_1(Lexer yym):base(yym) 	{yylval = yytext.Clone();}}
//%Suited+36
/// <exclude/>
public class Suited : TOKEN{ public override string yyname { get { return "Suited";}}
/// <exclude/>
public override int yynum { get { return 36; }}
/// <exclude/>
 public Suited(Lexer yyl):base(yyl) {}}
/// <exclude/>
public class Suited_1 : Suited {
  /// <exclude/>
  public Suited_1(Lexer yym):base(yym) 		{yylval = yytext.Clone();}}
//%Offsuit+38
/// <exclude/>
public class Offsuit : TOKEN{ public override string yyname { get { return "Offsuit";}}
/// <exclude/>
public override int yynum { get { return 38; }}
/// <exclude/>
 public Offsuit(Lexer yyl):base(yyl) {}}
/// <exclude/>
public class Offsuit_1 : Offsuit {
  /// <exclude/>
  public Offsuit_1(Lexer yym):base(yym) 	{yylval = yytext.Clone();}}
//%LE+40
/// <exclude/>
public class LE : TOKEN{ public override string yyname { get { return "LE";}}
/// <exclude/>
public override int yynum { get { return 40; }}
/// <exclude/>
 public LE(Lexer yyl):base(yyl) {}}
/// <exclude/>
public class LE_1 : LE {
  /// <exclude/>
  public LE_1(Lexer yym):base(yym) 			{yylval = yytext.Clone();}}
//%LT+42
/// <exclude/>
public class LT : TOKEN{ public override string yyname { get { return "LT";}}
/// <exclude/>
public override int yynum { get { return 42; }}
/// <exclude/>
 public LT(Lexer yyl):base(yyl) {}}
/// <exclude/>
public class LT_1 : LT {
  /// <exclude/>
  public LT_1(Lexer yym):base(yym) 			{yylval = yytext.Clone();}}
//%GE+44
/// <exclude/>
public class GE : TOKEN{ public override string yyname { get { return "GE";}}
/// <exclude/>
public override int yynum { get { return 44; }}
/// <exclude/>
 public GE(Lexer yyl):base(yyl) {}}
/// <exclude/>
public class GE_1 : GE {
  /// <exclude/>
  public GE_1(Lexer yym):base(yym) 			{yylval = yytext.Clone();}}
//%GT+46
/// <exclude/>
public class GT : TOKEN{ public override string yyname { get { return "GT";}}
/// <exclude/>
public override int yynum { get { return 46; }}
/// <exclude/>
 public GT(Lexer yyl):base(yyl) {}}
/// <exclude/>
public class GT_1 : GT {
  /// <exclude/>
  public GT_1(Lexer yym):base(yym) 			{yylval = yytext.Clone();}}
//%OR+48
/// <exclude/>
public class OR : TOKEN{ public override string yyname { get { return "OR";}}
/// <exclude/>
public override int yynum { get { return 48; }}
/// <exclude/>
 public OR(Lexer yyl):base(yyl) {}}
/// <exclude/>
public class OR_1 : OR {
  /// <exclude/>
  public OR_1(Lexer yym):base(yym) 			{yylval = yytext.Clone();}}
/// <exclude/>
public class OR_2 : OR {
  /// <exclude/>
  public OR_2(Lexer yym):base(yym) 			{yylval = yytext.Clone();}}
/// <exclude/>
public class OR_3 : OR {
  /// <exclude/>
  public OR_3(Lexer yym):base(yym) 			{yylval = yytext.Clone();}}
/// <exclude/>
public class OR_4 : OR {
  /// <exclude/>
  public OR_4(Lexer yym):base(yym) 			{yylval = yytext.Clone();}}
//%AND+53
/// <exclude/>
public class AND : TOKEN{ public override string yyname { get { return "AND";}}
/// <exclude/>
public override int yynum { get { return 53; }}
/// <exclude/>
 public AND(Lexer yyl):base(yyl) {}}
/// <exclude/>
public class AND_1 : AND {
  /// <exclude/>
  public AND_1(Lexer yym):base(yym) 		{yylval = yytext.Clone();}}
/// <exclude/>
public class AND_2 : AND {
  /// <exclude/>
  public AND_2(Lexer yym):base(yym) 		{yylval = yytext.Clone();}}
//%IN+56
/// <exclude/>
public class IN : TOKEN{ public override string yyname { get { return "IN";}}
/// <exclude/>
public override int yynum { get { return 56; }}
/// <exclude/>
 public IN(Lexer yyl):base(yyl) {}}
/// <exclude/>
public class IN_1 : IN {
  /// <exclude/>
  public IN_1(Lexer yym):base(yym) 			{yylval = yytext.Clone();}}
/// <exclude/>
public class AND_3 : AND {
  /// <exclude/>
  public AND_3(Lexer yym):base(yym) 		{yylval = yytext.Clone();}}
//%LParen+59
/// <exclude/>
public class LParen : TOKEN{ public override string yyname { get { return "LParen";}}
/// <exclude/>
public override int yynum { get { return 59; }}
/// <exclude/>
 public LParen(Lexer yyl):base(yyl) {}}
/// <exclude/>
public class LParen_1 : LParen {
  /// <exclude/>
  public LParen_1(Lexer yym):base(yym) 		{yylval = yytext.Clone();}}
//%RParen+61
/// <exclude/>
public class RParen : TOKEN{ public override string yyname { get { return "RParen";}}
/// <exclude/>
public override int yynum { get { return 61; }}
/// <exclude/>
 public RParen(Lexer yyl):base(yyl) {}}
/// <exclude/>
public class RParen_1 : RParen {
  /// <exclude/>
  public RParen_1(Lexer yym):base(yym) 		{yylval = yytext.Clone();}}
//%Group8+63
/// <exclude/>
public class Group8 : TOKEN{ public override string yyname { get { return "Group8";}}
/// <exclude/>
public override int yynum { get { return 63; }}
/// <exclude/>
 public Group8(Lexer yyl):base(yyl) {}}
/// <exclude/>
public class Group8_1 : Group8 {
  /// <exclude/>
  public Group8_1(Lexer yym):base(yym) 		{yylval = yytext.Clone();}}
//%Group7+65
/// <exclude/>
public class Group7 : TOKEN{ public override string yyname { get { return "Group7";}}
/// <exclude/>
public override int yynum { get { return 65; }}
/// <exclude/>
 public Group7(Lexer yyl):base(yyl) {}}
/// <exclude/>
public class Group7_1 : Group7 {
  /// <exclude/>
  public Group7_1(Lexer yym):base(yym) 		{yylval = yytext.Clone();}}
//%Group6+67
/// <exclude/>
public class Group6 : TOKEN{ public override string yyname { get { return "Group6";}}
/// <exclude/>
public override int yynum { get { return 67; }}
/// <exclude/>
 public Group6(Lexer yyl):base(yyl) {}}
/// <exclude/>
public class Group6_1 : Group6 {
  /// <exclude/>
  public Group6_1(Lexer yym):base(yym) 		{yylval = yytext.Clone();}}
//%Group5+69
/// <exclude/>
public class Group5 : TOKEN{ public override string yyname { get { return "Group5";}}
/// <exclude/>
public override int yynum { get { return 69; }}
/// <exclude/>
 public Group5(Lexer yyl):base(yyl) {}}
/// <exclude/>
public class Group5_1 : Group5 {
  /// <exclude/>
  public Group5_1(Lexer yym):base(yym) 		{yylval = yytext.Clone();}}
//%Group4+71
/// <exclude/>
public class Group4 : TOKEN{ public override string yyname { get { return "Group4";}}
/// <exclude/>
public override int yynum { get { return 71; }}
/// <exclude/>
 public Group4(Lexer yyl):base(yyl) {}}
/// <exclude/>
public class Group4_1 : Group4 {
  /// <exclude/>
  public Group4_1(Lexer yym):base(yym) 		{yylval = yytext.Clone();}}
//%Group3+73
/// <exclude/>
public class Group3 : TOKEN{ public override string yyname { get { return "Group3";}}
/// <exclude/>
public override int yynum { get { return 73; }}
/// <exclude/>
 public Group3(Lexer yyl):base(yyl) {}}
/// <exclude/>
public class Group3_1 : Group3 {
  /// <exclude/>
  public Group3_1(Lexer yym):base(yym) 		{yylval = yytext.Clone();}}
//%Group2+75
/// <exclude/>
public class Group2 : TOKEN{ public override string yyname { get { return "Group2";}}
/// <exclude/>
public override int yynum { get { return 75; }}
/// <exclude/>
 public Group2(Lexer yyl):base(yyl) {}}
/// <exclude/>
public class Group2_1 : Group2 {
  /// <exclude/>
  public Group2_1(Lexer yym):base(yym) 		{yylval = yytext.Clone();}}
//%Group1+77
/// <exclude/>
public class Group1 : TOKEN{ public override string yyname { get { return "Group1";}}
/// <exclude/>
public override int yynum { get { return 77; }}
/// <exclude/>
 public Group1(Lexer yyl):base(yyl) {}}
/// <exclude/>
public class Group1_1 : Group1 {
  /// <exclude/>
  public Group1_1(Lexer yym):base(yym) 		{yylval = yytext.Clone();}}
//%Group9+79
/// <exclude/>
public class Group9 : TOKEN{ public override string yyname { get { return "Group9";}}
/// <exclude/>
public override int yynum { get { return 79; }}
/// <exclude/>
 public Group9(Lexer yyl):base(yyl) {}}
/// <exclude/>
public class Group9_1 : Group9 {
  /// <exclude/>
  public Group9_1(Lexer yym):base(yym) 		{yylval = yytext.Clone();}}
/// <exclude/>
public class Group9_2 : Group9 {
  /// <exclude/>
  public Group9_2(Lexer yym):base(yym) 		{yylval = yytext.Clone();}}
//%|Exam
/// <exclude/>
public class yyExam : YyLexer {
/// <exclude/>
 public yyExam(ErrorHandler eh):base(eh) { arr = new int[] { 
101,4,6,52,0,
46,0,53,0,6,
102,4,16,117,0,
115,0,45,0,97,
0,115,0,99,0,
105,0,105,0,2,
0,103,5,27,7,
27,104,9,1,27,
3,94,0,105,5,
1,3,94,0,2,
1,7,26,106,9,
1,26,3,36,0,
107,5,1,3,36,
0,2,1,7,25,
108,9,1,25,3,
43,0,109,5,5,
3,124,0,2,1,
3,60,0,2,1,
3,61,0,2,1,
3,62,0,2,1,
3,43,0,2,1,
7,24,110,9,1,
24,3,34,0,111,
5,8,3,34,0,
2,1,3,38,0,
2,1,3,63,0,
2,1,3,33,0,
2,1,3,42,0,
2,1,3,44,0,
2,1,3,37,0,
2,1,3,46,0,
2,1,7,23,112,
9,1,23,3,187,
0,113,5,1,3,
187,0,2,1,7,
22,114,9,1,22,
3,171,0,115,5,
1,3,171,0,2,
1,7,21,116,9,
1,21,3,93,0,
117,5,2,3,93,
0,2,1,3,41,
0,2,1,7,20,
118,9,1,20,3,
91,0,119,5,2,
3,91,0,2,1,
3,40,0,2,1,
7,19,120,9,1,
19,3,173,0,121,
5,2,3,45,0,
2,1,3,173,0,
2,1,7,18,122,
9,1,18,3,95,
0,123,5,1,3,
95,0,2,1,7,
17,124,9,1,17,
3,0,224,125,5,
1,3,0,224,2,
1,7,15,126,9,
1,15,3,0,6,
127,5,1,3,0,
6,2,1,7,14,
128,9,1,14,3,
0,0,129,5,3,
3,13,0,2,1,
3,10,0,2,1,
3,0,0,2,1,
7,13,130,9,1,
13,3,41,32,131,
5,1,3,41,32,
2,1,7,12,132,
9,1,12,3,40,
32,133,5,1,3,
40,32,2,1,7,
11,134,9,1,11,
3,160,0,135,5,
2,3,160,0,2,
1,3,32,0,2,
1,7,10,136,9,
1,10,3,178,0,
137,5,1,3,178,
0,2,1,7,9,
138,9,1,9,3,
238,22,139,5,1,
3,238,22,2,1,
7,8,140,9,1,
8,3,96,6,141,
5,11,3,55,0,
2,1,3,48,0,
2,1,3,57,0,
2,1,3,50,0,
2,1,3,52,0,
2,1,3,96,6,
2,1,3,54,0,
2,1,3,56,0,
2,1,3,49,0,
2,1,3,51,0,
2,1,3,53,0,
2,1,7,7,142,
9,1,7,3,136,
4,143,5,1,3,
136,4,2,1,7,
6,144,9,1,6,
3,3,9,145,5,
1,3,3,9,2,
1,7,5,146,9,
1,5,3,0,3,
147,5,1,3,0,
3,2,1,7,4,
148,9,1,4,3,
187,1,149,5,1,
3,187,1,2,1,
7,3,150,9,1,
3,3,176,2,151,
5,1,3,176,2,
2,1,7,2,152,
9,1,2,3,197,
1,153,5,1,3,
197,1,2,1,7,
1,154,9,1,1,
3,108,0,155,5,
21,3,116,0,2,
1,3,111,0,2,
1,3,106,0,2,
1,3,101,0,2,
1,3,113,0,2,
1,3,108,0,2,
1,3,103,0,2,
1,3,98,0,2,
1,3,120,0,2,
1,3,115,0,2,
1,3,110,0,2,
1,3,105,0,2,
1,3,100,0,2,
1,3,117,0,2,
1,3,112,0,2,
1,3,107,0,2,
1,3,102,0,2,
1,3,97,0,2,
1,3,114,0,2,
1,3,104,0,2,
1,3,99,0,2,
1,7,0,156,9,
1,0,3,76,0,
157,5,21,3,84,
0,2,1,3,79,
0,2,1,3,74,
0,2,1,3,69,
0,2,1,3,81,
0,2,1,3,76,
0,2,1,3,71,
0,2,1,3,66,
0,2,1,3,88,
0,2,1,3,83,
0,2,1,3,78,
0,2,1,3,73,
0,2,1,3,68,
0,2,1,3,85,
0,2,1,3,80,
0,2,1,3,75,
0,2,1,3,70,
0,2,1,3,65,
0,2,1,3,82,
0,2,1,3,72,
0,2,1,3,67,
0,2,1,7,27,
2,0,158,5,1,
159,4,18,89,0,
89,0,73,0,78,
0,73,0,84,0,
73,0,65,0,76,
0,160,12,1,1172,
161,5,51,3,111,
0,162,12,1,4234,
163,5,4,3,102,
0,164,12,1,4303,
165,5,2,3,102,
0,166,12,1,4372,
167,5,2,3,115,
0,168,12,1,4434,
169,5,2,3,117,
0,170,12,1,4500,
171,5,2,3,105,
0,172,12,1,4564,
173,5,2,3,116,
0,174,12,1,4617,
175,5,2,3,100,
0,176,12,1,4682,
177,5,2,3,100,
0,178,12,1,4747,
179,5,0,180,11,
1,513,0,181,4,
18,79,0,102,0,
102,0,115,0,117,
0,105,0,116,0,
95,0,49,0,1,
-1,3,68,0,178,
0,182,4,0,1,
-1,3,69,0,176,
183,11,1,513,0,
181,1,-1,3,84,
0,174,0,182,1,
-1,3,73,0,172,
0,182,1,-1,3,
85,0,170,0,182,
1,-1,3,83,0,
168,0,182,1,-1,
3,70,0,166,0,
182,1,-1,3,114,
0,184,12,1,5057,
185,5,0,186,11,
1,595,0,187,4,
8,79,0,82,0,
95,0,49,0,1,
-1,3,82,0,184,
3,70,0,164,188,
11,1,513,0,181,
1,-1,3,113,0,
189,12,1,5175,190,
5,29,3,84,0,
191,12,1,3235,192,
5,5,3,111,0,
193,12,1,2948,194,
5,0,195,11,1,
14,0,196,4,18,
67,0,97,0,114,
0,100,0,49,0,
54,0,57,0,95,
0,49,0,1,-1,
3,79,0,193,3,
115,0,193,3,83,
0,193,3,42,0,
193,197,11,1,14,
0,196,1,-1,3,
97,0,191,3,116,
0,191,3,106,0,
191,3,54,0,191,
3,99,0,198,12,
1,3338,199,5,0,
200,11,1,2,0,
201,4,12,67,0,
97,0,114,0,100,
0,95,0,49,0,
1,-1,3,81,0,
191,3,100,0,198,
3,113,0,191,3,
56,0,191,3,51,
0,191,3,74,0,
191,3,88,0,202,
12,1,2440,203,5,
5,3,111,0,204,
12,1,2452,205,5,
0,206,11,1,34,
0,207,4,26,67,
0,97,0,114,0,
100,0,49,0,54,
0,57,0,87,0,
105,0,108,0,100,
0,95,0,49,0,
1,-1,3,79,0,
204,3,115,0,204,
3,83,0,204,3,
42,0,204,208,11,
1,34,0,207,1,
-1,3,83,0,198,
3,120,0,202,3,
115,0,198,3,63,
0,202,3,53,0,
191,3,104,0,198,
3,75,0,191,3,
65,0,191,3,107,
0,191,3,55,0,
191,3,50,0,191,
3,68,0,198,3,
72,0,198,3,67,
0,198,3,57,0,
191,3,52,0,191,
0,182,1,-1,3,
115,0,209,12,1,
8002,210,5,2,3,
117,0,211,12,1,
8068,212,5,2,3,
105,0,213,12,1,
8132,214,5,2,3,
116,0,215,12,1,
8185,216,5,2,3,
101,0,217,12,1,
8241,218,5,2,3,
100,0,219,12,1,
8306,220,5,0,221,
11,1,475,0,222,
4,16,83,0,117,
0,105,0,116,0,
101,0,100,0,95,
0,49,0,1,-1,
3,68,0,219,0,
182,1,-1,3,69,
0,217,0,182,1,
-1,3,84,0,215,
0,182,1,-1,3,
73,0,213,0,182,
1,-1,3,85,0,
211,223,11,1,475,
0,222,1,-1,3,
10,0,224,12,1,
2327,225,5,0,226,
11,1,1169,0,182,
1,-1,3,32,0,
224,3,38,0,227,
12,1,1651,228,5,
0,229,11,1,659,
0,230,4,10,65,
0,78,0,68,0,
95,0,50,0,1,
-1,3,40,0,231,
12,1,2133,232,5,
0,233,11,1,748,
0,234,4,16,76,
0,80,0,97,0,
114,0,101,0,110,
0,95,0,49,0,
1,-1,3,44,0,
235,12,1,1937,236,
5,0,237,11,1,
633,0,238,4,8,
79,0,82,0,95,
0,51,0,1,-1,
3,48,0,239,12,
1,3066,240,5,12,
3,55,0,239,3,
48,0,239,3,57,
0,239,3,50,0,
239,3,52,0,239,
3,54,0,239,3,
56,0,239,3,49,
0,239,3,51,0,
239,3,37,0,241,
12,1,2632,242,5,
0,243,11,1,54,
0,244,4,18,80,
0,101,0,114,0,
99,0,101,0,110,
0,116,0,95,0,
49,0,1,-1,3,
53,0,239,3,46,
0,245,12,1,2727,
246,5,10,3,55,
0,247,12,1,2763,
248,5,11,3,55,
0,247,3,48,0,
247,3,57,0,247,
3,50,0,247,3,
52,0,247,3,54,
0,247,3,56,0,
247,3,49,0,247,
3,51,0,247,3,
37,0,241,3,53,
0,247,249,11,1,
86,0,250,4,16,
78,0,117,0,109,
0,98,0,101,0,
114,0,95,0,49,
0,1,-1,3,48,
0,247,3,57,0,
247,3,50,0,247,
3,52,0,247,3,
54,0,247,3,56,
0,247,3,49,0,
247,3,51,0,247,
3,53,0,247,0,
182,1,-1,251,11,
1,86,0,250,1,
-1,3,50,0,252,
12,1,2430,253,5,
33,3,37,0,241,
3,97,0,191,3,
116,0,191,3,106,
0,191,3,54,0,
254,12,1,2936,255,
5,17,3,42,0,
193,3,37,0,241,
3,79,0,193,3,
111,0,193,3,54,
0,239,3,49,0,
239,3,56,0,239,
3,51,0,239,3,
46,0,245,3,83,
0,193,3,115,0,
193,3,53,0,239,
3,48,0,239,3,
55,0,239,3,50,
0,239,3,57,0,
239,3,52,0,239,
256,11,1,14,0,
196,1,-1,3,49,
0,239,3,104,0,
198,3,84,0,191,
3,100,0,198,3,
113,0,191,3,56,
0,254,3,51,0,
254,3,46,0,245,
3,88,0,202,3,
83,0,198,3,120,
0,202,3,115,0,
198,3,63,0,202,
3,53,0,254,3,
48,0,239,3,75,
0,191,3,74,0,
191,3,107,0,191,
3,55,0,254,3,
50,0,254,3,65,
0,191,3,68,0,
198,3,67,0,198,
3,72,0,198,3,
81,0,191,3,57,
0,254,3,52,0,
254,3,99,0,198,
257,11,1,86,0,
250,1,-1,3,52,
0,252,3,54,0,
252,3,56,0,252,
3,60,0,258,12,
1,1270,259,5,1,
3,61,0,260,12,
1,1275,261,5,0,
262,11,1,573,0,
263,4,8,76,0,
69,0,95,0,49,
0,1,-1,264,11,
1,579,0,265,4,
8,76,0,84,0,
95,0,49,0,1,
-1,3,62,0,266,
12,1,1460,267,5,
1,3,61,0,268,
12,1,1465,269,5,
0,270,11,1,584,
0,271,4,8,71,
0,69,0,95,0,
49,0,1,-1,272,
11,1,590,0,273,
4,8,71,0,84,
0,95,0,49,0,
1,-1,3,66,0,
274,12,1,7718,275,
5,2,3,117,0,
276,12,1,7784,277,
5,2,3,116,0,
278,12,1,7837,279,
5,0,280,11,1,
403,0,281,4,10,
66,0,117,0,116,
0,95,0,49,0,
1,-1,3,84,0,
278,0,182,1,-1,
3,85,0,276,0,
182,1,-1,3,74,
0,189,3,78,0,
282,12,1,8567,283,
5,2,3,111,0,
284,12,1,8621,285,
5,2,3,116,0,
286,12,1,8674,287,
5,0,288,11,1,
382,0,289,4,10,
78,0,111,0,116,
0,95,0,49,0,
1,-1,3,84,0,
286,0,182,1,-1,
3,79,0,284,0,
182,1,-1,3,80,
0,290,12,1,10451,
291,5,2,3,97,
0,292,12,1,10521,
293,5,2,3,105,
0,294,12,1,10585,
295,5,2,3,114,
0,296,12,1,10656,
297,5,2,3,101,
0,298,12,1,10712,
299,5,2,3,100,
0,300,12,1,10777,
301,5,0,302,11,
1,344,0,303,4,
12,80,0,97,0,
105,0,114,0,95,
0,49,0,1,-1,
3,68,0,300,0,
182,1,-1,3,69,
0,298,304,11,1,
344,0,303,1,-1,
3,82,0,296,0,
182,1,-1,3,73,
0,294,0,182,1,
-1,3,65,0,292,
0,182,1,-1,3,
84,0,305,12,1,
3481,306,5,31,3,
84,0,191,3,79,
0,307,12,1,3535,
308,5,0,309,11,
1,294,0,310,4,
8,84,0,111,0,
95,0,50,0,1,
-1,3,97,0,191,
3,116,0,191,3,
111,0,307,3,106,
0,191,3,54,0,
191,3,99,0,198,
3,81,0,191,3,
100,0,198,3,113,
0,191,3,56,0,
191,3,51,0,191,
3,74,0,191,3,
88,0,202,3,83,
0,198,3,120,0,
202,3,115,0,198,
3,63,0,202,3,
53,0,191,3,104,
0,311,12,1,3647,
312,5,2,3,114,
0,313,12,1,3718,
314,5,2,3,111,
0,315,12,1,3772,
316,5,2,3,117,
0,317,12,1,3838,
318,5,2,3,103,
0,319,12,1,3897,
320,5,2,3,104,
0,321,12,1,3969,
322,5,0,323,11,
1,304,0,324,4,
8,84,0,111,0,
95,0,51,0,1,
-1,3,72,0,321,
0,182,1,-1,3,
71,0,319,0,182,
1,-1,3,85,0,
317,0,182,1,-1,
3,79,0,315,0,
182,1,-1,3,82,
0,313,325,11,1,
2,0,201,1,-1,
3,75,0,191,3,
65,0,191,3,107,
0,191,3,55,0,
191,3,50,0,191,
3,68,0,198,3,
72,0,311,3,67,
0,198,3,57,0,
191,3,52,0,191,
0,182,1,-1,3,
98,0,274,3,117,
0,326,12,1,9980,
327,5,2,3,78,
0,328,12,1,10043,
329,5,2,3,105,
0,330,12,1,10107,
331,5,2,3,111,
0,332,12,1,10161,
333,5,2,3,78,
0,334,12,1,10224,
335,5,0,336,11,
1,605,0,337,4,
8,79,0,82,0,
95,0,50,0,1,
-1,3,110,0,334,
0,182,1,-1,3,
79,0,332,0,182,
1,-1,3,73,0,
330,0,182,1,-1,
3,110,0,328,0,
182,1,-1,3,106,
0,189,3,110,0,
282,3,112,0,290,
3,116,0,305,3,
13,0,224,3,124,
0,338,12,1,1175,
339,5,0,340,11,
1,638,0,341,4,
8,79,0,82,0,
95,0,52,0,1,
-1,3,33,0,342,
12,1,1841,343,5,
0,344,11,1,398,
0,345,4,10,78,
0,111,0,116,0,
95,0,50,0,1,
-1,3,41,0,346,
12,1,2037,347,5,
0,348,11,1,753,
0,349,4,16,82,
0,80,0,97,0,
114,0,101,0,110,
0,95,0,49,0,
1,-1,3,45,0,
350,12,1,2228,351,
5,0,352,11,1,
289,0,353,4,8,
84,0,111,0,95,
0,49,0,1,-1,
3,49,0,239,3,
51,0,252,3,53,
0,252,3,55,0,
252,3,57,0,252,
3,63,0,354,12,
1,1746,355,5,0,
356,11,1,111,0,
357,4,10,65,0,
110,0,121,0,95,
0,49,0,1,-1,
3,65,0,358,12,
1,11018,359,5,31,
3,110,0,360,12,
1,11081,361,5,2,
3,100,0,362,12,
1,11146,363,5,0,
364,11,1,643,0,
365,4,10,65,0,
78,0,68,0,95,
0,49,0,1,-1,
3,68,0,362,0,
182,1,-1,3,97,
0,191,3,116,0,
191,3,106,0,191,
3,54,0,191,3,
99,0,198,3,84,
0,191,3,100,0,
198,3,113,0,191,
3,56,0,191,3,
51,0,191,3,74,
0,191,3,88,0,
202,3,83,0,198,
3,78,0,360,3,
120,0,202,3,115,
0,198,3,63,0,
202,3,53,0,191,
3,104,0,198,3,
75,0,191,3,65,
0,191,3,107,0,
191,3,55,0,191,
3,50,0,191,3,
68,0,198,3,67,
0,198,3,72,0,
198,3,81,0,191,
3,57,0,191,3,
52,0,191,0,182,
1,-1,3,67,0,
366,12,1,11303,367,
5,2,3,111,0,
368,12,1,11357,369,
5,2,3,78,0,
370,12,1,11420,371,
5,2,3,78,0,
372,12,1,11483,373,
5,2,3,101,0,
374,12,1,11539,375,
5,2,3,67,0,
376,12,1,11612,377,
5,2,3,116,0,
378,12,1,11665,379,
5,2,3,101,0,
380,12,1,11721,381,
5,2,3,100,0,
382,12,1,11786,383,
5,0,384,11,1,
419,0,385,4,22,
67,0,111,0,110,
0,110,0,101,0,
99,0,116,0,101,
0,100,0,95,0,
49,0,1,-1,3,
68,0,382,0,182,
1,-1,3,69,0,
380,0,182,1,-1,
3,84,0,378,0,
182,1,-1,3,99,
0,376,0,182,1,
-1,3,69,0,374,
0,182,1,-1,3,
110,0,372,0,182,
1,-1,3,110,0,
370,0,182,1,-1,
3,79,0,368,386,
11,1,419,0,385,
1,-1,3,71,0,
387,12,1,5273,388,
5,15,3,55,0,
389,12,1,5309,390,
5,0,391,11,1,
797,0,392,4,16,
71,0,114,0,111,
0,117,0,112,0,
55,0,95,0,49,
0,1,-1,3,110,
0,393,12,1,6182,
394,5,2,3,111,
0,395,12,1,6236,
396,5,2,3,78,
0,397,12,1,6299,
398,5,2,3,101,
0,399,12,1,6355,
400,5,0,401,11,
1,1109,0,402,4,
16,71,0,114,0,
111,0,117,0,112,
0,57,0,95,0,
50,0,1,-1,3,
69,0,399,0,182,
1,-1,3,110,0,
397,0,182,1,-1,
3,79,0,395,403,
11,1,1109,0,402,
1,-1,3,57,0,
404,12,1,5405,405,
5,0,406,11,1,
1070,0,407,4,16,
71,0,114,0,111,
0,117,0,112,0,
57,0,95,0,49,
0,1,-1,3,50,
0,408,12,1,5500,
409,5,0,410,11,
1,992,0,411,4,
16,71,0,114,0,
111,0,117,0,112,
0,50,0,95,0,
49,0,1,-1,3,
82,0,412,12,1,
7318,413,5,2,3,
111,0,414,12,1,
7372,415,5,2,3,
117,0,416,12,1,
7438,417,5,2,3,
112,0,418,12,1,
7505,419,5,11,3,
55,0,389,3,110,
0,393,3,57,0,
404,3,50,0,408,
3,52,0,420,12,
1,5595,421,5,0,
422,11,1,914,0,
423,4,16,71,0,
114,0,111,0,117,
0,112,0,52,0,
95,0,49,0,1,
-1,3,54,0,424,
12,1,5691,425,5,
0,426,11,1,836,
0,427,4,16,71,
0,114,0,111,0,
117,0,112,0,54,
0,95,0,49,0,
1,-1,3,78,0,
393,3,56,0,428,
12,1,5786,429,5,
0,430,11,1,758,
0,431,4,16,71,
0,114,0,111,0,
117,0,112,0,56,
0,95,0,49,0,
1,-1,3,49,0,
432,12,1,5881,433,
5,0,434,11,1,
1031,0,435,4,16,
71,0,114,0,111,
0,117,0,112,0,
49,0,95,0,49,
0,1,-1,3,51,
0,436,12,1,5976,
437,5,0,438,11,
1,953,0,439,4,
16,71,0,114,0,
111,0,117,0,112,
0,51,0,95,0,
49,0,1,-1,3,
53,0,440,12,1,
6071,441,5,0,442,
11,1,875,0,443,
4,16,71,0,114,
0,111,0,117,0,
112,0,53,0,95,
0,49,0,1,-1,
0,182,1,-1,3,
80,0,418,0,182,
1,-1,3,85,0,
416,0,182,1,-1,
3,79,0,414,0,
182,1,-1,3,52,
0,420,3,114,0,
412,3,54,0,424,
3,78,0,393,3,
56,0,428,3,49,
0,432,3,65,0,
444,12,1,6565,445,
5,2,3,112,0,
446,12,1,6632,447,
5,5,3,112,0,
448,12,1,6981,449,
5,2,3,101,0,
450,12,1,7037,451,
5,2,3,100,0,
452,12,1,7102,453,
5,3,3,49,0,
454,12,1,6770,455,
5,0,456,11,1,
154,0,457,4,18,
71,0,97,0,112,
0,112,0,101,0,
100,0,49,0,95,
0,49,0,1,-1,
3,50,0,458,12,
1,6671,459,5,0,
460,11,1,199,0,
461,4,18,71,0,
97,0,112,0,112,
0,101,0,100,0,
50,0,95,0,49,
0,1,-1,3,51,
0,462,12,1,6865,
463,5,0,464,11,
1,244,0,465,4,
18,71,0,97,0,
112,0,112,0,101,
0,100,0,51,0,
95,0,49,0,1,
-1,466,11,1,116,
0,467,4,16,71,
0,97,0,112,0,
112,0,101,0,100,
0,95,0,49,0,
1,-1,3,68,0,
452,0,182,1,-1,
3,69,0,450,0,
182,1,-1,3,80,
0,448,3,49,0,
454,3,50,0,458,
3,51,0,462,468,
11,1,116,0,467,
1,-1,3,80,0,
446,0,182,1,-1,
3,97,0,444,3,
51,0,436,3,53,
0,440,0,182,1,
-1,3,73,0,469,
12,1,8850,470,5,
2,3,78,0,471,
12,1,8913,472,5,
2,3,116,0,473,
12,1,8966,474,5,
2,3,101,0,475,
12,1,9022,476,5,
2,3,114,0,477,
12,1,9093,478,5,
2,3,115,0,479,
12,1,9155,480,5,
2,3,101,0,481,
12,1,9211,482,5,
2,3,67,0,483,
12,1,9284,484,5,
2,3,116,0,485,
12,1,9337,486,5,
2,3,105,0,487,
12,1,9401,488,5,
2,3,111,0,489,
12,1,9455,490,5,
2,3,78,0,491,
12,1,9518,492,5,
0,493,11,1,674,
0,494,4,10,65,
0,78,0,68,0,
95,0,51,0,1,
-1,3,110,0,491,
0,182,1,-1,3,
79,0,489,0,182,
1,-1,3,73,0,
487,495,11,1,674,
0,494,1,-1,3,
84,0,485,0,182,
1,-1,3,99,0,
483,0,182,1,-1,
3,69,0,481,0,
182,1,-1,3,83,
0,479,0,182,1,
-1,3,82,0,477,
0,182,1,-1,3,
69,0,475,0,182,
1,-1,3,84,0,
473,496,11,1,664,
0,497,4,8,73,
0,78,0,95,0,
49,0,1,-1,3,
110,0,471,0,182,
1,-1,3,75,0,
189,3,79,0,162,
3,81,0,189,3,
83,0,209,3,85,
0,326,3,97,0,
358,3,99,0,366,
3,103,0,387,3,
105,0,469,3,107,
0,189,0,182,1,
-1,498,5,79,439,
499,10,439,1,74,
500,4,4,76,0,
84,0,501,10,500,
1,42,502,4,12,
71,0,114,0,111,
0,117,0,112,0,
50,0,503,10,502,
1,75,341,504,10,
341,1,52,427,505,
10,427,1,68,196,
506,10,196,1,6,
507,4,14,71,0,
97,0,112,0,112,
0,101,0,100,0,
50,0,508,10,507,
1,19,509,4,12,
71,0,114,0,111,
0,117,0,112,0,
53,0,510,10,509,
1,69,511,4,4,
71,0,84,0,512,
10,511,1,46,513,
4,4,73,0,78,
0,514,10,513,1,
56,407,515,10,407,
1,80,516,4,12,
71,0,114,0,111,
0,117,0,112,0,
56,0,517,10,516,
1,63,518,4,12,
76,0,80,0,97,
0,114,0,101,0,
110,0,519,10,518,
1,59,324,520,10,
324,1,26,521,4,
6,78,0,111,0,
116,0,522,10,521,
1,29,222,523,10,
222,1,37,411,524,
10,411,1,76,525,
4,12,82,0,80,
0,97,0,114,0,
101,0,110,0,526,
10,525,1,61,234,
527,10,234,1,60,
528,4,12,71,0,
114,0,111,0,117,
0,112,0,49,0,
529,10,528,1,77,
530,4,6,65,0,
78,0,68,0,531,
10,530,1,53,263,
532,10,263,1,41,
244,533,10,244,1,
10,467,534,10,467,
1,16,443,535,10,
443,1,70,281,536,
10,281,1,33,537,
4,12,71,0,114,
0,111,0,117,0,
112,0,52,0,538,
10,537,1,71,271,
539,10,271,1,45,
230,540,10,230,1,
55,365,541,10,365,
1,54,431,542,10,
431,1,64,543,4,
14,71,0,97,0,
112,0,112,0,101,
0,100,0,51,0,
544,10,543,1,21,
349,545,10,349,1,
62,303,546,10,303,
1,28,547,4,8,
80,0,97,0,105,
0,114,0,548,10,
547,1,27,250,549,
10,250,1,12,385,
550,10,385,1,35,
457,551,10,457,1,
18,552,4,14,79,
0,102,0,102,0,
115,0,117,0,105,
0,116,0,553,10,
552,1,38,423,554,
10,423,1,72,555,
4,6,66,0,117,
0,116,0,556,10,
555,1,32,557,4,
12,78,0,117,0,
109,0,98,0,101,
0,114,0,558,10,
557,1,11,559,4,
14,80,0,101,0,
114,0,99,0,101,
0,110,0,116,0,
560,10,559,1,9,
461,561,10,461,1,
20,494,562,10,494,
1,58,337,563,10,
337,1,50,564,4,
8,67,0,97,0,
114,0,100,0,565,
10,564,1,3,566,
4,4,76,0,69,
0,567,10,566,1,
40,568,4,4,79,
0,82,0,569,10,
568,1,48,570,4,
6,65,0,110,0,
121,0,571,10,570,
1,13,572,4,4,
71,0,69,0,573,
10,572,1,44,574,
4,18,67,0,111,
0,110,0,110,0,
101,0,99,0,116,
0,101,0,100,0,
575,10,574,1,34,
353,576,10,353,1,
24,577,4,12,71,
0,114,0,111,0,
117,0,112,0,55,
0,578,10,577,1,
65,357,579,10,357,
1,14,265,580,10,
265,1,43,273,581,
10,273,1,47,465,
582,10,465,1,22,
497,583,10,497,1,
57,207,584,10,207,
1,8,238,585,10,
238,1,51,289,586,
10,289,1,30,345,
587,10,345,1,31,
588,4,14,71,0,
97,0,112,0,112,
0,101,0,100,0,
49,0,589,10,588,
1,17,181,590,10,
181,1,39,435,591,
10,435,1,78,592,
4,12,71,0,114,
0,111,0,117,0,
112,0,51,0,593,
10,592,1,73,392,
594,10,392,1,66,
402,595,10,402,1,
81,596,4,12,71,
0,114,0,111,0,
117,0,112,0,54,
0,597,10,596,1,
67,187,598,10,187,
1,49,310,599,10,
310,1,25,600,4,
12,71,0,114,0,
111,0,117,0,112,
0,57,0,601,10,
600,1,79,602,4,
12,83,0,117,0,
105,0,116,0,101,
0,100,0,603,10,
602,1,36,604,4,
12,71,0,97,0,
112,0,112,0,101,
0,100,0,605,10,
604,1,15,606,4,
14,67,0,97,0,
114,0,100,0,49,
0,54,0,57,0,
607,10,606,1,5,
608,4,22,67,0,
97,0,114,0,100,
0,49,0,54,0,
57,0,87,0,105,
0,108,0,100,0,
609,10,608,1,7,
201,610,10,201,1,
4,611,4,4,84,
0,111,0,612,10,
611,1,23,613,5,
0,0};
 new Tfactory(this,"Group3_1",new TCreator(Group3_1_factory));
 new Tfactory(this,"LT",new TCreator(LT_factory));
 new Tfactory(this,"Group2",new TCreator(Group2_factory));
 new Tfactory(this,"OR_4",new TCreator(OR_4_factory));
 new Tfactory(this,"Group6_1",new TCreator(Group6_1_factory));
 new Tfactory(this,"Card169_1",new TCreator(Card169_1_factory));
 new Tfactory(this,"Gapped2",new TCreator(Gapped2_factory));
 new Tfactory(this,"Group5",new TCreator(Group5_factory));
 new Tfactory(this,"GT",new TCreator(GT_factory));
 new Tfactory(this,"IN",new TCreator(IN_factory));
 new Tfactory(this,"Group9_1",new TCreator(Group9_1_factory));
 new Tfactory(this,"Group8",new TCreator(Group8_factory));
 new Tfactory(this,"LParen",new TCreator(LParen_factory));
 new Tfactory(this,"To_3",new TCreator(To_3_factory));
 new Tfactory(this,"Not",new TCreator(Not_factory));
 new Tfactory(this,"Suited_1",new TCreator(Suited_1_factory));
 new Tfactory(this,"Group2_1",new TCreator(Group2_1_factory));
 new Tfactory(this,"RParen",new TCreator(RParen_factory));
 new Tfactory(this,"LParen_1",new TCreator(LParen_1_factory));
 new Tfactory(this,"Group1",new TCreator(Group1_factory));
 new Tfactory(this,"AND",new TCreator(AND_factory));
 new Tfactory(this,"LE_1",new TCreator(LE_1_factory));
 new Tfactory(this,"Percent_1",new TCreator(Percent_1_factory));
 new Tfactory(this,"Gapped_1",new TCreator(Gapped_1_factory));
 new Tfactory(this,"Group5_1",new TCreator(Group5_1_factory));
 new Tfactory(this,"But_1",new TCreator(But_1_factory));
 new Tfactory(this,"Group4",new TCreator(Group4_factory));
 new Tfactory(this,"GE_1",new TCreator(GE_1_factory));
 new Tfactory(this,"AND_2",new TCreator(AND_2_factory));
 new Tfactory(this,"AND_1",new TCreator(AND_1_factory));
 new Tfactory(this,"Group8_1",new TCreator(Group8_1_factory));
 new Tfactory(this,"Gapped3",new TCreator(Gapped3_factory));
 new Tfactory(this,"RParen_1",new TCreator(RParen_1_factory));
 new Tfactory(this,"Pair_1",new TCreator(Pair_1_factory));
 new Tfactory(this,"Pair",new TCreator(Pair_factory));
 new Tfactory(this,"Number_1",new TCreator(Number_1_factory));
 new Tfactory(this,"Connected_1",new TCreator(Connected_1_factory));
 new Tfactory(this,"Gapped1_1",new TCreator(Gapped1_1_factory));
 new Tfactory(this,"Offsuit",new TCreator(Offsuit_factory));
 new Tfactory(this,"Group4_1",new TCreator(Group4_1_factory));
 new Tfactory(this,"But",new TCreator(But_factory));
 new Tfactory(this,"Number",new TCreator(Number_factory));
 new Tfactory(this,"Percent",new TCreator(Percent_factory));
 new Tfactory(this,"Gapped2_1",new TCreator(Gapped2_1_factory));
 new Tfactory(this,"AND_3",new TCreator(AND_3_factory));
 new Tfactory(this,"OR_2",new TCreator(OR_2_factory));
 new Tfactory(this,"Card",new TCreator(Card_factory));
 new Tfactory(this,"LE",new TCreator(LE_factory));
 new Tfactory(this,"OR",new TCreator(OR_factory));
 new Tfactory(this,"Any",new TCreator(Any_factory));
 new Tfactory(this,"GE",new TCreator(GE_factory));
 new Tfactory(this,"Connected",new TCreator(Connected_factory));
 new Tfactory(this,"To_1",new TCreator(To_1_factory));
 new Tfactory(this,"Group7",new TCreator(Group7_factory));
 new Tfactory(this,"Any_1",new TCreator(Any_1_factory));
 new Tfactory(this,"LT_1",new TCreator(LT_1_factory));
 new Tfactory(this,"GT_1",new TCreator(GT_1_factory));
 new Tfactory(this,"Gapped3_1",new TCreator(Gapped3_1_factory));
 new Tfactory(this,"IN_1",new TCreator(IN_1_factory));
 new Tfactory(this,"Card169Wild_1",new TCreator(Card169Wild_1_factory));
 new Tfactory(this,"OR_3",new TCreator(OR_3_factory));
 new Tfactory(this,"Not_1",new TCreator(Not_1_factory));
 new Tfactory(this,"Not_2",new TCreator(Not_2_factory));
 new Tfactory(this,"Gapped1",new TCreator(Gapped1_factory));
 new Tfactory(this,"Offsuit_1",new TCreator(Offsuit_1_factory));
 new Tfactory(this,"Group1_1",new TCreator(Group1_1_factory));
 new Tfactory(this,"Group3",new TCreator(Group3_factory));
 new Tfactory(this,"Group7_1",new TCreator(Group7_1_factory));
 new Tfactory(this,"Group9_2",new TCreator(Group9_2_factory));
 new Tfactory(this,"Group6",new TCreator(Group6_factory));
 new Tfactory(this,"OR_1",new TCreator(OR_1_factory));
 new Tfactory(this,"To_2",new TCreator(To_2_factory));
 new Tfactory(this,"Group9",new TCreator(Group9_factory));
 new Tfactory(this,"Suited",new TCreator(Suited_factory));
 new Tfactory(this,"Gapped",new TCreator(Gapped_factory));
 new Tfactory(this,"Card169",new TCreator(Card169_factory));
 new Tfactory(this,"Card169Wild",new TCreator(Card169Wild_factory));
 new Tfactory(this,"Card_1",new TCreator(Card_1_factory));
 new Tfactory(this,"To",new TCreator(To_factory));
}
/// <exclude/>
public static object Group3_1_factory(Lexer yyl) { return new Group3_1(yyl);}
/// <exclude/>
public static object LT_factory(Lexer yyl) { return new LT(yyl);}
/// <exclude/>
public static object Group2_factory(Lexer yyl) { return new Group2(yyl);}
/// <exclude/>
public static object OR_4_factory(Lexer yyl) { return new OR_4(yyl);}
/// <exclude/>
public static object Group6_1_factory(Lexer yyl) { return new Group6_1(yyl);}
/// <exclude/>
public static object Card169_1_factory(Lexer yyl) { return new Card169_1(yyl);}
/// <exclude/>
public static object Gapped2_factory(Lexer yyl) { return new Gapped2(yyl);}
/// <exclude/>
public static object Group5_factory(Lexer yyl) { return new Group5(yyl);}
/// <exclude/>
public static object GT_factory(Lexer yyl) { return new GT(yyl);}
/// <exclude/>
public static object IN_factory(Lexer yyl) { return new IN(yyl);}
/// <exclude/>
public static object Group9_1_factory(Lexer yyl) { return new Group9_1(yyl);}
/// <exclude/>
public static object Group8_factory(Lexer yyl) { return new Group8(yyl);}
/// <exclude/>
public static object LParen_factory(Lexer yyl) { return new LParen(yyl);}
/// <exclude/>
public static object To_3_factory(Lexer yyl) { return new To_3(yyl);}
/// <exclude/>
public static object Not_factory(Lexer yyl) { return new Not(yyl);}
/// <exclude/>
public static object Suited_1_factory(Lexer yyl) { return new Suited_1(yyl);}
/// <exclude/>
public static object Group2_1_factory(Lexer yyl) { return new Group2_1(yyl);}
/// <exclude/>
public static object RParen_factory(Lexer yyl) { return new RParen(yyl);}
/// <exclude/>
public static object LParen_1_factory(Lexer yyl) { return new LParen_1(yyl);}
/// <exclude/>
public static object Group1_factory(Lexer yyl) { return new Group1(yyl);}
/// <exclude/>
public static object AND_factory(Lexer yyl) { return new AND(yyl);}
/// <exclude/>
public static object LE_1_factory(Lexer yyl) { return new LE_1(yyl);}
/// <exclude/>
public static object Percent_1_factory(Lexer yyl) { return new Percent_1(yyl);}
/// <exclude/>
public static object Gapped_1_factory(Lexer yyl) { return new Gapped_1(yyl);}
/// <exclude/>
public static object Group5_1_factory(Lexer yyl) { return new Group5_1(yyl);}
/// <exclude/>
public static object But_1_factory(Lexer yyl) { return new But_1(yyl);}
/// <exclude/>
public static object Group4_factory(Lexer yyl) { return new Group4(yyl);}
/// <exclude/>
public static object GE_1_factory(Lexer yyl) { return new GE_1(yyl);}
/// <exclude/>
public static object AND_2_factory(Lexer yyl) { return new AND_2(yyl);}
/// <exclude/>
public static object AND_1_factory(Lexer yyl) { return new AND_1(yyl);}
/// <exclude/>
public static object Group8_1_factory(Lexer yyl) { return new Group8_1(yyl);}
/// <exclude/>
public static object Gapped3_factory(Lexer yyl) { return new Gapped3(yyl);}
/// <exclude/>
public static object RParen_1_factory(Lexer yyl) { return new RParen_1(yyl);}
/// <exclude/>
public static object Pair_1_factory(Lexer yyl) { return new Pair_1(yyl);}
/// <exclude/>
public static object Pair_factory(Lexer yyl) { return new Pair(yyl);}
/// <exclude/>
public static object Number_1_factory(Lexer yyl) { return new Number_1(yyl);}
/// <exclude/>
public static object Connected_1_factory(Lexer yyl) { return new Connected_1(yyl);}
/// <exclude/>
public static object Gapped1_1_factory(Lexer yyl) { return new Gapped1_1(yyl);}
/// <exclude/>
public static object Offsuit_factory(Lexer yyl) { return new Offsuit(yyl);}
/// <exclude/>
public static object Group4_1_factory(Lexer yyl) { return new Group4_1(yyl);}
/// <exclude/>
public static object But_factory(Lexer yyl) { return new But(yyl);}
/// <exclude/>
public static object Number_factory(Lexer yyl) { return new Number(yyl);}
/// <exclude/>
public static object Percent_factory(Lexer yyl) { return new Percent(yyl);}
/// <exclude/>
public static object Gapped2_1_factory(Lexer yyl) { return new Gapped2_1(yyl);}
/// <exclude/>
public static object AND_3_factory(Lexer yyl) { return new AND_3(yyl);}
/// <exclude/>
public static object OR_2_factory(Lexer yyl) { return new OR_2(yyl);}
/// <exclude/>
public static object Card_factory(Lexer yyl) { return new Card(yyl);}
/// <exclude/>
public static object LE_factory(Lexer yyl) { return new LE(yyl);}
/// <exclude/>
public static object OR_factory(Lexer yyl) { return new OR(yyl);}
/// <exclude/>
public static object Any_factory(Lexer yyl) { return new Any(yyl);}
/// <exclude/>
public static object GE_factory(Lexer yyl) { return new GE(yyl);}
/// <exclude/>
public static object Connected_factory(Lexer yyl) { return new Connected(yyl);}
/// <exclude/>
public static object To_1_factory(Lexer yyl) { return new To_1(yyl);}
/// <exclude/>
public static object Group7_factory(Lexer yyl) { return new Group7(yyl);}
/// <exclude/>
public static object Any_1_factory(Lexer yyl) { return new Any_1(yyl);}
/// <exclude/>
public static object LT_1_factory(Lexer yyl) { return new LT_1(yyl);}
/// <exclude/>
public static object GT_1_factory(Lexer yyl) { return new GT_1(yyl);}
/// <exclude/>
public static object Gapped3_1_factory(Lexer yyl) { return new Gapped3_1(yyl);}
/// <exclude/>
public static object IN_1_factory(Lexer yyl) { return new IN_1(yyl);}
/// <exclude/>
public static object Card169Wild_1_factory(Lexer yyl) { return new Card169Wild_1(yyl);}
/// <exclude/>
public static object OR_3_factory(Lexer yyl) { return new OR_3(yyl);}
/// <exclude/>
public static object Not_1_factory(Lexer yyl) { return new Not_1(yyl);}
/// <exclude/>
public static object Not_2_factory(Lexer yyl) { return new Not_2(yyl);}
/// <exclude/>
public static object Gapped1_factory(Lexer yyl) { return new Gapped1(yyl);}
/// <exclude/>
public static object Offsuit_1_factory(Lexer yyl) { return new Offsuit_1(yyl);}
/// <exclude/>
public static object Group1_1_factory(Lexer yyl) { return new Group1_1(yyl);}
/// <exclude/>
public static object Group3_factory(Lexer yyl) { return new Group3(yyl);}
/// <exclude/>
public static object Group7_1_factory(Lexer yyl) { return new Group7_1(yyl);}
/// <exclude/>
public static object Group9_2_factory(Lexer yyl) { return new Group9_2(yyl);}
/// <exclude/>
public static object Group6_factory(Lexer yyl) { return new Group6(yyl);}
/// <exclude/>
public static object OR_1_factory(Lexer yyl) { return new OR_1(yyl);}
/// <exclude/>
public static object To_2_factory(Lexer yyl) { return new To_2(yyl);}
/// <exclude/>
public static object Group9_factory(Lexer yyl) { return new Group9(yyl);}
/// <exclude/>
public static object Suited_factory(Lexer yyl) { return new Suited(yyl);}
/// <exclude/>
public static object Gapped_factory(Lexer yyl) { return new Gapped(yyl);}
/// <exclude/>
public static object Card169_factory(Lexer yyl) { return new Card169(yyl);}
/// <exclude/>
public static object Card169Wild_factory(Lexer yyl) { return new Card169Wild(yyl);}
/// <exclude/>
public static object Card_1_factory(Lexer yyl) { return new Card_1(yyl);}
/// <exclude/>
public static object To_factory(Lexer yyl) { return new To(yyl);}
/// <exclude/>
public override TOKEN OldAction(Lexer yym,ref string yytext,int action, ref bool reject) {
  switch(action) {
  case -1: break;
   case 1169: ;
      break;
  }
  return null;
}}
/// <exclude/>
public class Exam:Lexer {
/// <exclude/>
public Exam():base(new yyExam(new ErrorHandler(false))) {}
/// <exclude/>
public Exam(ErrorHandler eh):base(new yyExam(eh)) {}
/// <exclude/>
public Exam(YyLexer tks):base(tks){}

 }
