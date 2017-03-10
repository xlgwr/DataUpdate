---------------------------------------------------------------------------------------会议记录
--COMPANY
select top 100 a.name,a.nameDescSearch from EntitysAll a where ttype='Officer'
and a.nameDesc like '%[Cc][Oo][.]%' ;

--LIMITED
select  top 100 a.name,a.nameDescSearch  from EntitysAll a where ttype='Officer'
and a.nameDesc like '%[Ll][Tt][Dd]%' ;

select top 100 a.name,a.nameDescSearch from EntitysAll a where ttype='Officer'
and a.nameDesc like '%  %' ;

----INC. --Incorporation
select top 100 a.name,a.nameDescSearch from EntitysAll a where ttype='Officer'
and a.nameDesc like '%[Ii][Nn][Cc][.]%' ;

----& --And
select top 100 a.name,a.nameDescSearch from EntitysAll a where ttype='Officer'
and a.nameDesc like '%[&]%' ;

----@ --空格
select top 100 a.name,a.nameDescSearch from EntitysAll a where ttype='Officer'
and a.nameDesc like '%[@?-,.()'']%' ;
select top 100 a.name,a.nameDescSearch from EntitysAll a where ttype='Officer'
and a.nameDesc like '%['']%' ;

--CO. ----COMPANY
update EntitysAll set nameDescSearch=replace(nameDescSearch,N'CO.',N'COMPANY');
--LTD. ---LIMITED
update EntitysAll set nameDescSearch=replace(nameDescSearch,N'LTD.',N'LIMITED');
update EntitysAll set nameDescSearch=replace(nameDescSearch,N'LTD',N'LIMITED');
---INC. --Incorporation
update EntitysAll set nameDescSearch=replace(nameDescSearch,N'INC.',N'Incorporation');
----& --And
update EntitysAll set nameDescSearch=replace(nameDescSearch,N'&',N'And');
-----@ --控格
update EntitysAll set nameDescSearch=replace(nameDescSearch,N'@',N' ');
-----? --控格
update EntitysAll set nameDescSearch=replace(nameDescSearch,N'?',N' ');
-----+ --控格
--update EntitysAll set nameDescSearch=replace(nameDescSearch,N'+',N' ');
-----减号- --控格
update EntitysAll set nameDescSearch=replace(nameDescSearch,N'-',N' ');
-----, --控格
update EntitysAll set nameDescSearch=replace(nameDescSearch,N',',N' ');
--多个空格
update EntitysAll set nameDescSearch=replace(nameDescSearch,N'  ',N' ');

--Officer,address,entity,intermediary
select top 1000 a.name,a.nameDesc,a.ttype,a.nameDescSearch from EntitysAll a --where ttype='Officer'

--------------------------------------------------------------------------------------------------------------------文档1
--来次客户文档。缩写前事有空格，才改写
--Court Leads - Defendant & Plaintiff's Address Abbreviation 
select top 100 a.name,a.nameDescSearch from EntitysAll a where --ttype='Officer' and 
a.nameDesc like N'% Ave %' 
--1.Ave-->Avenue
update EntitysAll set nameDescSearch=replace(nameDescSearch,N' Ave ',N' Avenue ');
--2.Comml-->Commerical
update EntitysAll set nameDescSearch=replace(nameDescSearch,N' Comml ',N' Commerical ');
--3./F-->Floor
update EntitysAll set nameDescSearch=replace(nameDescSearch,N'/F',N' Floor ');
--4.Gdn-->Gardens No exec
update EntitysAll set nameDescSearch=replace(nameDescSearch,N' Gdn ',N' Gardens ');
--5.HK-->Hong Kong 
update EntitysAll set nameDescSearch=replace(nameDescSearch,N' HK ',N' Hong Kong ');

--6.House	Hse
update EntitysAll set nameDescSearch=replace(nameDescSearch,N' Hse ',N' House ');
--7.Industrial	Indl
update EntitysAll set nameDescSearch=replace(nameDescSearch,N' Indl ',N' Industrial ');
--8.Kowloon	Kln
update EntitysAll set nameDescSearch=replace(nameDescSearch,N' Kln ',N' Kowloon ');
--9.New Territories	-->NT
update EntitysAll set nameDescSearch=replace(nameDescSearch,N' NT ',N' New Territories ');
--10. Wang Tau Hom South Road	Wang Tau Hom S Rd
update EntitysAll set nameDescSearch=replace(nameDescSearch,N'Wang Tau Hom S Rd',N' Wang Tau Hom South Road ');
--11. Wang Tau Hom East Road	Wang Tau Hom E Rd
update EntitysAll set nameDescSearch=replace(nameDescSearch,N'Wang Tau Hom E Rd',N' Wang Tau Hom East Road ');
--12. Wang Tau Hom North Road	Wang Tau Hom N Rd
update EntitysAll set nameDescSearch=replace(nameDescSearch,N'Wang Tau Hom N Rd',N' Wang Tau Hom North Road ');
--12. Wang Yip Street East	Wang Yip St E
update EntitysAll set nameDescSearch=replace(nameDescSearch,N'Wang Yip St E',N' Wang Yip Street East ');
--12. Wang Yip Street South	Wang Yip St S
update EntitysAll set nameDescSearch=replace(nameDescSearch,N'Wang Yip St S',N' Wang Yip Street South ');
--10. South	S
update EntitysAll set nameDescSearch=replace(nameDescSearch,N' S ',N' South ');
--11. East	E
update EntitysAll set nameDescSearch=replace(nameDescSearch,N' E ',N' East ');
--12. North	N
update EntitysAll set nameDescSearch=replace(nameDescSearch,N' N ',N' North ');
--13.Road	Rd
update EntitysAll set nameDescSearch=replace(nameDescSearch,N' Rd ',N' Road ');
--14.Room	Rm
update EntitysAll set nameDescSearch=replace(nameDescSearch,N' Rm ',N' Room ');
--15.Street	St
update EntitysAll set nameDescSearch=replace(nameDescSearch,N' St ',N' Street ');

--16.Terrace	Terr
update EntitysAll set nameDescSearch=replace(nameDescSearch,N' Terr ',N' Terrace ');
--17.Tower	Twr
update EntitysAll set nameDescSearch=replace(nameDescSearch,N' Twr ',N' Tower ');
--18.Village	Vill
update EntitysAll set nameDescSearch=replace(nameDescSearch,N' Vill ',N' Village ');
--19.Centre Ctr
update EntitysAll set nameDescSearch=replace(nameDescSearch,N' Ctr ',N' Centre ');
--20.Estate Est
update EntitysAll set nameDescSearch=replace(nameDescSearch,N' Est ',N' Estate ');
--21.Flat 8, 20/F., ABC Building	2008 ABC Bldg
update EntitysAll set nameDescSearch=replace(nameDescSearch,N'2008 ABC Bldg',N' Flat 8, 20/F., ABC Building ');
--22.Room L, 18 Floor, DEF Building	18L DEF Bldg
update EntitysAll set nameDescSearch=replace(nameDescSearch,N'18L DEF Bldg',N' Room L, 18 Floor, DEF Building ');
--23.Building Bldg
update EntitysAll set nameDescSearch=replace(nameDescSearch,N' Bldg ',N' Building ');
-------------------------------------------------------------------------------------------
-------------------------------------------------------------------------------------------文档2
--Creditcheck.com.hk - Explanatory Note
--1	ADVG	Advertising

update EntitysAll set nameDescSearch=replace(nameDescSearch,N' ADVG ',N' Advertising ');
--2	ASS	Association
update EntitysAll set nameDescSearch=replace(nameDescSearch,N' ASS ',N' Association ');
--3	ASSOCS	Associates
update EntitysAll set nameDescSearch=replace(nameDescSearch,N' ASSOCS ',N' Associates ');
--4	BLDG	Building
update EntitysAll set nameDescSearch=replace(nameDescSearch,N' BLDG ',N' Building ');
--5	CO	Company
update EntitysAll set nameDescSearch=replace(nameDescSearch,N' CO ',N' Company ');
--6	COMM/ COML	Commercial
update EntitysAll set nameDescSearch=replace(nameDescSearch,N' COMM ',N' Commercial ');
update EntitysAll set nameDescSearch=replace(nameDescSearch,N' COML ',N' Commercial ');
--7	CONSTN	Construction
update EntitysAll set nameDescSearch=replace(nameDescSearch,N' CONSTN ',N' Construction ');
--8	CORP	Corporation
update EntitysAll set nameDescSearch=replace(nameDescSearch,N' CORP ',N' Corporation ');

--9	CTR	Centre
update EntitysAll set nameDescSearch=replace(nameDescSearch,N' CTR ',N' Centre ');
--10	DEV 	Development
update EntitysAll set nameDescSearch=replace(nameDescSearch,N' DEV ',N' Development ');
--11	DIST	Distribution
update EntitysAll set nameDescSearch=replace(nameDescSearch,N' DIST ',N' Distribution ');
--12	ELEC	Electric
update EntitysAll set nameDescSearch=replace(nameDescSearch,N' ELEC ',N' Electric ');
--13	ELECTL	Electrical
update EntitysAll set nameDescSearch=replace(nameDescSearch,N' ELECTL ',N' Electrical ');
--14	ELECTRS	Electronics
update EntitysAll set nameDescSearch=replace(nameDescSearch,N' ELECTRS ',N' Electronics ');
--15	EMB'D	Embroidery
update EntitysAll set nameDescSearch=replace(nameDescSearch,N' EMB''D ',N' Embroidery ');
--16	ENG	Engineering
update EntitysAll set nameDescSearch=replace(nameDescSearch,N' ENG ',N' Engineering ');
--17	ENTP	Enterprise
update EntitysAll set nameDescSearch=replace(nameDescSearch,N' ENTP ',N' Enterprise ');
--18	ENTPS	Enterprises
update EntitysAll set nameDescSearch=replace(nameDescSearch,N' ENTPS ',N' Enterprises ');
--19	EQPT	Equipment
update EntitysAll set nameDescSearch=replace(nameDescSearch,N' EQPT ',N' Equipment ');
--20	EST	Estate
update EntitysAll set nameDescSearch=replace(nameDescSearch,N' EST ',N' Estate ');
--21	EXP	Export
update EntitysAll set nameDescSearch=replace(nameDescSearch,N' EXP ',N' Export ');
--22	EXPS	Exports
update EntitysAll set nameDescSearch=replace(nameDescSearch,N' EXPS ',N' Exports ');
--23	FTY	Factory
update EntitysAll set nameDescSearch=replace(nameDescSearch,N' FTY ',N' Factory ');
--24	GMT	Garment
update EntitysAll set nameDescSearch=replace(nameDescSearch,N' GMT ',N' Garment ');
--25	GMTS	Garments
update EntitysAll set nameDescSearch=replace(nameDescSearch,N' GMTS ',N' Garments ');
--26	HLDGS	Holdings
update EntitysAll set nameDescSearch=replace(nameDescSearch,N' HLDGS ',N' Holdings ');
--27	IMP	Import
update EntitysAll set nameDescSearch=replace(nameDescSearch,N' IMP ',N' Import ');
--28	IMPS	Imports
update EntitysAll set nameDescSearch=replace(nameDescSearch,N' IMPS ',N' Imports ');
--29	INC	Incorporation
update EntitysAll set nameDescSearch=replace(nameDescSearch,N' INC ',N' Incorporation ');
--30	IND'L	Industrial
update EntitysAll set nameDescSearch=replace(nameDescSearch,N' IND''L ',N' Industrial ');
--31	INDS	Industries
update EntitysAll set nameDescSearch=replace(nameDescSearch,N' INDS ',N' Industries ');
--32	INT'L	International
update EntitysAll set nameDescSearch=replace(nameDescSearch,N' INT''L ',N' International ');
--33	INV / INVT	Investment
update EntitysAll set nameDescSearch=replace(nameDescSearch,N' INV ',N' Investment ');
update EntitysAll set nameDescSearch=replace(nameDescSearch,N' INVT ',N' Investment ');
--34	INVTS	Investments
update EntitysAll set nameDescSearch=replace(nameDescSearch,N' INVTS ',N' Investments ');
--35	LTD	Limited
update EntitysAll set nameDescSearch=replace(nameDescSearch,N' LTD ',N' Limited ');
--36	MACH	Machine
update EntitysAll set nameDescSearch=replace(nameDescSearch,N' MACH ',N' Machine ');
--37	MACHY	Machinery
update EntitysAll set nameDescSearch=replace(nameDescSearch,N' MACHY ',N' Machinery ');
--38	MATRL	Material
update EntitysAll set nameDescSearch=replace(nameDescSearch,N' MATRL ',N' Material ');
--39	MFG	Manufacturing
update EntitysAll set nameDescSearch=replace(nameDescSearch,N' MFG ',N' Manufacturing ');
--40	MFR	Manufacturer
update EntitysAll set nameDescSearch=replace(nameDescSearch,N' MFR ',N' Manufacturer ');
--41	MFY	Manufactory
update EntitysAll set nameDescSearch=replace(nameDescSearch,N' MFY ',N' Manufactory ');
--42	MGT	Management
update EntitysAll set nameDescSearch=replace(nameDescSearch,N' MGT ',N' Management ');
--43	No.	Number
update EntitysAll set nameDescSearch=replace(nameDescSearch,N' No.',N' Number ');
--44	PDT	Product
update EntitysAll set nameDescSearch=replace(nameDescSearch,N' PDT ',N' Product ');
--45	PDTN	Production
update EntitysAll set nameDescSearch=replace(nameDescSearch,N' PDTN ',N' Production ');
--46	PDTS	Products
update EntitysAll set nameDescSearch=replace(nameDescSearch,N' PDTS ',N' Products ');
--47	PRTG	Printing
update EntitysAll set nameDescSearch=replace(nameDescSearch,N' PRTG ',N' Printing ');
--48	PTE	Private
update EntitysAll set nameDescSearch=replace(nameDescSearch,N' PTE ',N' Private ');
--49	REST	Restaurant
update EntitysAll set nameDescSearch=replace(nameDescSearch,N' REST ',N' Restaurant ');
--50	SEV	Service
update EntitysAll set nameDescSearch=replace(nameDescSearch,N' SEV ',N' Service ');
--51	SEVS	Services
update EntitysAll set nameDescSearch=replace(nameDescSearch,N' SEVS ',N' Services ');
--52	SYMS	Systems
update EntitysAll set nameDescSearch=replace(nameDescSearch,N' SYMS ',N' Systems ');
--53	TECH.	Technology
update EntitysAll set nameDescSearch=replace(nameDescSearch,N' TECH.',N' Technology ');
--54	TRANSPTN	Transportation
update EntitysAll set nameDescSearch=replace(nameDescSearch,N' TRANSPTN ',N' Transportation ');
--55	TRDG	Trading
update EntitysAll set nameDescSearch=replace(nameDescSearch,N' TRDG ',N' Trading ');
--56	TXL	Textile
update EntitysAll set nameDescSearch=replace(nameDescSearch,N' TXL ',N' Textile ');
--57	TXLS	Textiles
update EntitysAll set nameDescSearch=replace(nameDescSearch,N' TXLS ',N' Textiles ');

update EntitysAll set nameDescSearch=replace(nameDescSearch,N'  ',N' ');
--59	Unauthorized Structure at R/F 1-3 Hok Ling Street, Kln			Unauthorized Structures of 1 Hok Ling Street	
update EntitysAll set nameDescSearch=replace(nameDescSearch,N'Unauthorized Structure at R/F 1-3 Hok Ling Street, Kln',N' Unauthorized Structures of 1 Hok Ling Street ');
update EntitysAll set nameDescSearch=replace(nameDescSearch,N'Unauthorized Structure at R Floor 1 3 Hok Ling Street, Kln',N' Unauthorized Structures of 1 Hok Ling Street ');

--60	Incorporated Owners of 1-3 Hok Ling Street			Incorporated Owners of Hok Ling Street	
update EntitysAll set nameDescSearch=replace(nameDescSearch,N' Incorporated Owners of 1-3 Hok Ling Street ',N' Incorporated Owners of Hok Ling Street ');
update EntitysAll set nameDescSearch=replace(nameDescSearch,N' Incorporated Owners of 1 3 Hok Ling Street ',N' Incorporated Owners of Hok Ling Street ');
--61	景富、可富、祈富及嘉富業主立案法團			景富樓業主立案法團	
update EntitysAll set nameDescSearch=replace(nameDescSearch,N'景富、可富、祈富及嘉富業主立案法團',N' 景富樓業主立案法團 ');
--62黃埔街22 22A 24號及必嘉街88 90 90A號業主立案法團
update EntitysAll set nameDescSearch=replace(nameDescSearch,N'黃埔街22 22A 24號及必嘉街88 90 90A號業主立案法團',N' 黃埔街業主立案法團 ');

--------From web-sitte
--b : The English School of Foundation 
--: English School of Foundation
update EntitysAll set nameDescSearch=replace(nameDescSearch,N'The English School of Foundation',N'English School of Foundation');
--: A B C Ltd 
--: ABC Limited
update EntitysAll set nameDescSearch=replace(nameDescSearch,N' A B C ',N'ABC');
update EntitysAll set nameDescSearch=replace(nameDescSearch,N'A. B. C.',N'ABC');
update EntitysAll set nameDescSearch=replace(nameDescSearch,N'H.K.',N'Hong Kong');
update EntitysAll set nameDescSearch=replace(nameDescSearch,N'(FE)',N'(Far East)');
update EntitysAll set nameDescSearch=replace(nameDescSearch,N'China/Hong',N'China Hong');
update EntitysAll set nameDescSearch=replace(nameDescSearch,N'The Watsen''s',N'Watsen''s');
update EntitysAll set nameDescSearch=replace(nameDescSearch,N'.com',N' com');
update EntitysAll set nameDescSearch=replace(nameDescSearch,N'Chan T M David',N'Chan David TM');
update EntitysAll set nameDescSearch=replace(nameDescSearch,N'Apple Wong Chan Mei Ching',N'Wong Chan Mei Ching Apple');

--add 

update EntitysAll set nameDescSearch=replace(nameDescSearch,N' PTE.',N' Private ');
--多个空格
update EntitysAll set nameDescSearch=replace(nameDescSearch,N'    ',N' ');
update EntitysAll set nameDescSearch=replace(nameDescSearch,N'   ',N' ');
update EntitysAll set nameDescSearch=replace(nameDescSearch,N'  ',N' ');
