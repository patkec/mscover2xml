mscover2xml
===========

Converts data from MS Test coverage files (VS 2012) to XML and/or in-memory structure.

XML output is similar to <a href="http://msdn.microsoft.com/en-us/library/microsoft.visualstudio.coverage.analysis.coverageds.aspx">CoverageDS</a> 
XML output, but with the advantage of enumerating block coverage on demand, thus minimizing memory footprint for larger coverage dumps. 
This is based on Peter Huene's <a href="http://blogs.msdn.com/b/phuene/archive/2009/12/01/programmatic-coverage-analysis-in-visual-studio-2010.aspx">Programmatic Coverage Analysis in Visual Studio 2010</a>.