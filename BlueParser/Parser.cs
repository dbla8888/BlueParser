using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlueParser
{
    //TODO: Refactor to remove internal state and make thread safe
    public class Parser<T>
    {
        protected IDictionary<string, ISymbol<T>> SymbolTable { get; }
        protected IEnumerator<ISymbol<T>> Tokens { get; set; }
        protected ISymbol<T> CurrentToken { get; set; }
        protected int CurrentPosition { get; set; }
        protected int CurrentLine { get; set; }

        public Parser(IEnumerable<ISymbol<T>> symbols)
        {
            this.SymbolTable = symbols?.ToDictionary(x => x.Id) ?? new Dictionary<string, ISymbol<T>>();
        }

        public virtual IParseResult<T> Parse(string @string)
        {
            this.Tokens = this.Tokenize(@string);
            this.CurrentToken = this.GetNextToken();
            return ParseResult<T>.GetResult(() => this.Parse(0));
        }

        internal T Parse(int rightBindingPower)
        {
            var token = this.CurrentToken;
            this.CurrentToken = this.GetNextToken();
            var left = this.NullDenotation(token);
            while(rightBindingPower < this.CurrentToken.BindingPower)
            {
                token = this.CurrentToken;
                this.CurrentToken = this.GetNextToken();
                left = this.LeftDenotation(token, left);
            }
            return left;
        }

        private T NullDenotation(ISymbol<T> token)
        {
            if (token.Nud == null) { throw new ParseException(this.CurrentLine, this.CurrentPosition, $"Syntax Error at line:{this.CurrentLine}, Pos:{this.CurrentPosition}; Id:{token.Id}"); }
            return token.Nud(this);
        }

        private T LeftDenotation(ISymbol<T> token, T left)
        {
            if (token.Led == null) { throw new ParseException(this.CurrentLine, this.CurrentPosition, $"Syntax Error at line:{this.CurrentLine}, Pos:{this.CurrentPosition}; Id:{token.Id}"); }
            return token.Led(this, left);
        }

        private ISymbol<T> GetNextToken()
        {
            return this.Tokens != null && this.Tokens.MoveNext() ?
                this.Tokens.Current :
                Symbol<T>.End;
        }

        //TODO: look this over for clarity improvements
        private IEnumerator<ISymbol<T>> Tokenize(string input)
        {
            this.CurrentPosition = 0;
            this.CurrentLine = 0;
            while(CurrentPosition < input.Length)
            {
                var initialPosition = this.CurrentPosition;
                ISymbol<T> match = null;
                foreach(var symbol in this.SymbolTable.Values)
                {
                    //check whether the given symbol matches at the current position
                    int j = symbol.Scanner(input, initialPosition);
                    // save the longest match with the greatest binding power
                    if(j > CurrentPosition || match != null && j == CurrentPosition && symbol.BindingPower > match.BindingPower)
                    {
                        match = symbol;
                        CurrentPosition = j;
                    }
                }
                if(initialPosition == CurrentPosition)
                {
                    throw new ParseException(this.CurrentLine, this.CurrentPosition, $"Unknown symbol at position {this.CurrentPosition}: {input[CurrentPosition]}");
                }else if( match != null && !match.Ignore)
                {
                    // return a token for the longest match
                    yield return match.TokenClone(input.Substring(initialPosition, this.CurrentPosition - initialPosition));
                    this.CurrentLine += input.Substring(initialPosition, CurrentPosition - initialPosition).Count(x => x.Equals('\n'));
                }
            }
            yield return Symbol<T>.End;
        }

        public void Advance(string id)
        {
            if (!string.IsNullOrEmpty(id) && this.CurrentToken.Id != id) { throw new ParseException(this.CurrentLine, this.CurrentPosition, $"Expected '{id}', found {CurrentToken.Id} as line:{this.CurrentLine}, position:{this.CurrentPosition}"); }
            //TODO: is this as intended?
            this.CurrentToken = GetNextToken();
        }
    }
}
